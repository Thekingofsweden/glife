{File, Directory} = require 'atom'
fs = require 'fs'

module.exports =
class QspDecoder
    constructor: (@data, @file) ->
        @decodeFile()

    decodeFile: ->
        prepare = @prepare()
        if !prepare
            return
        version = @readLine(prepare)
        console.log @toString(version.line)
        password = @readLine(version.next)
        console.log @toString @decode password.line
        locations = @readLine password.next
        locationsNumber = (@toString @decode locations.line).valueOf()

        next = locations.next
        builder = new ProjectBuilder(@config)
        for i in [0...locationsNumber]
            location = @readLocation next
            next = location.next
            builder.createLocation location

    prepare: ->
        @config = {}
        @config.ucs2le = @data[1] == 0
        @config.crlf = @data[7 * if @config.ucs2le then 2 else 1] == 13
        @config.encodedPath = @file.getPath()
        check = @readLine(0, 9)

        if @toString(check.line) == 'QSPGAME'
            return check.next
        else
            return 0

    readLocation: (position) ->
        name = @readLine position
        description = @readLine name.next
        code = @readLine description.next
        actions = @readLine code.next

        actionsNumber = (@toString @decode actions.line).valueOf()
        next = actions.next
        actions = []
        for i in [0...actionsNumber]
            action = @readAction next
            next = action.next
            actions.push(action)

        name: @toString @decode name.line
        description: @toString @decode description.line
        code: @toString @decode code.line
        actions: actions
        next: next

    readAction: (position) ->
        picture = @readLine position
        name = @readLine picture.next
        code = @readLine name.next

        picture: @toString @decode picture.line
        name: @toString @decode name.line
        code: @toString @decode code.line
        next: code.next

    readLine: (start, max) ->
        counter = start
        line = []
        that = @
        check = ->
            if that.config.crlf
                return line[line.length - 1] == 10 and line[line.length - 2] == 13
            else
                return line[line.length - 1] == 10

        while !check() and counter < @data.length
            line.push(@readSymbol(counter))
            counter += if @config.ucs2le then 2 else 1
            if max and line.length > max
                break

        line: if line.length > 0 then line[0...if @config.crlf then -2 else -1] else []
        next: counter

    readSymbol: (position) ->
        code = @data[position]
        code += @data[position + 1] * 256 if @config.ucs2le
        return code

    decode: (line) ->
        for i in [0...line.length]
            line[i] += 5
        return line

    toString: (line) ->
        result = ""
        step = 65535
        for i in [0...line.length] by step
            result += String.fromCharCode.apply(@, line[i...i + step])
        return result

class ProjectBuilder
    constructor: (@config) ->
        @directoryStack = []
        @root = new Directory(@config.encodedPath[0...@config.encodedPath.lastIndexOf('.')])
        try
            fs.mkdirSync(@root.getPath(), 0o755)
        catch error

    createLocation: (location) ->
        fs.writeFileSync("#{@root.getPath()}/#{location.name}.qsrc", location.code, {mode: 0o644})

    createDirectoryRecursive: (directory, callback) ->
        if directory.getParent().exists()
            that = @
            directory.create(0o755).then(() ->
                if that.directoryStack.length > 0
                    that.createDirectoryRecursive(that.directoryStack.pop(), callback)
                else
                    callback()
            )
        else
            @directoryStack.push(directory)
            createDirectoryRecursive(directory.getParent(), callback)

    write: (path, data) ->
