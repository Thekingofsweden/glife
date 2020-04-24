using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Analyser
{
    public class Location : Common
    {
        QSPGameCode m_gamecode;
        string m_name;
        int m_line;
        bool m_start; //Стартовая локация
        List<string> location_codeline_text;
        Dictionary<int, int> location_codeline_level;

        int currentLine;
        bool currentRoot;

        enum ControlBlockType
        {
            None,
            SL_If,
            SL_Else,
            SL_Act,
            ML_If,
            ML_Else,
            ML_Act,
            IfCondition,
            ElseIfCondition,
            ActCondition
        }
        
        struct ControlLine
        {
            public ControlBlockType controlBlock;
            public int line;
        }

        public Location(QSPGameCode code, string name, int line, bool start)
        {
            m_gamecode = code;
            m_name = name;
            m_line = line;
            m_start = start;
            location_codeline_text = null;
            location_codeline_level = null;
        }

        public string GetName()
        {
            return m_name;
        }

        public int GetLine()
        {
            return m_line;
        }

        public int GetCodeLinesCount()
        {
            return location_codeline_text.Count;
        }

        public void GetCodeLine(int lineNum, ref string text, ref int level)
        {
            //Нужно для утилиты форматирования
            int realNum = lineNum + 1;
            text = location_codeline_text[lineNum];
            if (location_codeline_level.ContainsKey(realNum))
                level = location_codeline_level[realNum];
            else
                level = INVALID_INDEX;
            return;
        }

        new void SubmitError(string text, int line)
        {
            if (currentRoot)
                Common.SubmitError(text, line);
            else
                Common.SubmitError(text, currentLine);
        }

        new void SubmitWarning(string text, int line)
        {
            if (currentRoot)
                Common.SubmitWarning(text, line);
            else
                Common.SubmitWarning(text, currentLine);
        }

        bool qspCompileExprPushOpCode(ref int[] opStack, ref int opSp, ref int[] argStack, ref int argSp, int opCode)
        {
            if ((opSp == QSP_STACKSIZE - 1) || (argSp == QSP_STACKSIZE - 1))
            {
                SubmitError("Quite complicated expression", currentLine);
                return false;
            }
            opStack[++opSp] = opCode;
            argStack[++argSp] = (opCode < (int)QspFunctionType.First_Function) ? qspOps[opCode].MinArgsCount : 0;
            return true;
        }

        bool qspAppendToCompiled(ref int itemsCount)
        {
            if (itemsCount == QSP_MAXITEMS)
            {
                SubmitError("Quite many arguments in the expression", currentLine);
                return false;
            }
            ++itemsCount;
            return true;
        }

        public bool ParseLocation(List<string> code, bool root)
        {
            int quote = (int)QuoteType.None;
            if (root)
            {
                currentRoot = true;
                currentLine = 0;
                location_codeline_text = new List<string>(code);
                location_codeline_level = new Dictionary<int, int>();
            }

            //Склеиваем строки
            List<int> lineIndexes = new List<int>();
            List<string> escapedLines = new List<string>();
            int linesCount = code.Count;
            int oldLine = 0;
            int curLine = 0;
            int curlyLevel = 0;
            string extraLine = "";
            while (curLine < linesCount)
            {
                string line = code.ToArray()[curLine];
                int pos = 0;
                while (pos < line.Length)
                {
                    char c = line[pos];
                    if (c == '\'')
                    {
                        //Апостроф
                        if (quote == (int)QuoteType.None)
                        {
                            quote = (int)QuoteType.Single;
                        }
                        else if (quote == (int)QuoteType.Single)
                        {
                            if ((pos + 1 < line.Length) && (line[pos + 1] == '\''))
                            {
                                //Экранированный апостроф
                                pos++;
                            }
                            else
                            {
                                quote = (int)QuoteType.None;
                            }
                        }
                    }
                    else if (c == '"')
                    {
                        //Кавычка
                        if (quote == (int)QuoteType.None)
                        {
                            quote = (int)QuoteType.Double;
                        }
                        else if (quote == (int)QuoteType.Double)
                        {
                            if ((pos + 1 < line.Length) && (line[pos + 1] == '"'))
                            {
                                //Экранированная кавычка
                                pos++;
                            }
                            else
                            {
                                quote = (int)QuoteType.None;
                            }
                        }
                    }
                    else if (quote == (int)QuoteType.None)
                    {
                        if (c == '{')
                        {
                            curlyLevel++;
                        }
                        else if (c == '}')
                        {
                            if (curlyLevel == 0)
                            {
                                SubmitError("Closing curly brace without opening one", oldLine + 1);
                                return false;
                            }
                            curlyLevel--;
                        }
                    }
                    pos++;
                }

                if ((curlyLevel > 0) || (quote != (int)QuoteType.None) || line.EndsWith(" _"))
                {
                    if (line.EndsWith(" _"))
                        line = line.Substring(0, line.Length - 2);
                    extraLine += line;
                    if (curlyLevel > 0)
                        extraLine += "\n";//Вставляем разделитель для строк, потом по нему "разобьем" строки
                    if (curLine == linesCount - 1)
                    {
                        lineIndexes.Add(oldLine + 1);
                        string escaped = extraLine + line;
                        escapedLines.Add(escaped.Trim(WhiteSpace));
                    }
                }
                else
                {
                    lineIndexes.Add(oldLine + 1);
                    string escaped = extraLine + line;
                    escapedLines.Add(escaped.Trim(WhiteSpace));
                    oldLine = curLine + 1;
                    extraLine = "";
                }
                curLine++;
            }
            //Проверка на незакрытые в конце локации многострочные строки
            if (quote != (int)QuoteType.None)
            {
                SubmitError("Unclosed string", oldLine + 1);
                return false;
            }
            //Проверка на незакрытые в конце локации фигурные скобки
            if (curlyLevel > 0)
            {
                SubmitError("Unclosed curly bracket", oldLine + 1);
                return false;
            }

            //TODO
            // FOR
            // перечисления asd|12|ddd

            //Разбираем код

            int lineNum = 0;
            int linePlainNum = 0;
            string block = "";
            string quotedText = "";
            int controlBlock = (int)ControlBlockType.None;
            Stack<ControlLine> ControlStack = new Stack<ControlLine>();
            List<string> blockGroup = new List<string>();
            List<bool> blockInGroupIsText = new List<bool>();

            int groupInnerCounter = 0;

            bool blockStarted = false;
            bool blockCompleted = false;
            bool blockGroupStarted = false;
            bool blockGroupCompleted = false;
            bool quotedTextCompleted = false;

            string rawText = "";
            string debugText = "";
            foreach (string line in escapedLines)
            {
                lineNum = lineIndexes[linePlainNum];
                linePlainNum++;

                int startIndentLevel = 0;
                int endIndentLevel = 0;
                if (currentRoot)
                {
                    currentLine = lineNum;
                    //Вычисляем уровень вложенности(для утилиты форматирования кода)
                    int stackSize = ControlStack.Count;
                    if (stackSize > 0)
                        startIndentLevel = stackSize;
                }

                int pos = 0;
                int firstSymbolPos = INVALID_INDEX;
                int blockStartPos = INVALID_INDEX;
                int lastIfPos = INVALID_INDEX;
                int lastActPos = INVALID_INDEX;
                int bracketLevel = 0;

                bool comment = false;
                bool label = false;
                string labelText = "";
                bool singleBlockGroup = false;
                
                //Разбор строки
                while (pos < line.Length)
                {
                    bool appendDelimiter = false;
                    String delimiterToAppend = "";
                    char c = line[pos];
                    if ((c.ToString().IndexOfAny(WhiteSpace) != 0) && (firstSymbolPos == INVALID_INDEX))
                    {
                        firstSymbolPos = pos;
                    }
                    if (c == '\'')
                    {
                        //Апостроф
                        if (quote == (int)QuoteType.None)
                        {
                            quote = (int)QuoteType.Single;

                            quotedText = "";
                            if (blockStarted && !comment)
                                blockCompleted = true;

                            //для отладки
                            if (rawText.Length == 0)
                                rawText += c;
                            else
                                rawText += Environment.NewLine + c;
                        }
                        else if (quote == (int)QuoteType.Single)
                        {
                            if ((pos + 1 < line.Length) && (line[pos + 1] == '\''))
                            {
                                //Экранированный апостроф
                                pos++;
                                quotedText += c;
                            }
                            else
                            {
                                quote = (int)QuoteType.None;
                                if (!comment && !label)
                                    quotedTextCompleted = true;
                            }

                            //для отладки
                            rawText += c;
                        }
                        else
                        {
                            quotedText += c;

                            //для отладки
                            rawText += c;
                        }
                    }
                    else if (c == '"')
                    {
                        //Кавычка
                        if (quote == (int)QuoteType.None)
                        {
                            quote = (int)QuoteType.Double;

                            quotedText = "";
                            if (blockStarted && !comment)
                                blockCompleted = true;

                            //для отладки
                            if (rawText.Length == 0)
                                rawText += c;
                            else
                                rawText += Environment.NewLine + c;
                        }
                        else if (quote == (int)QuoteType.Double)
                        {
                            if ((pos + 1 < line.Length) && (line[pos + 1] == '"'))
                            {
                                //Экранированная кавычка
                                pos++;
                                quotedText += c;
                            }
                            else
                            {
                                quote = (int)QuoteType.None;
                                if (!comment && !label)
                                    quotedTextCompleted = true;
                            }

                            //для отладки
                            rawText += c;
                        }
                        else
                        {
                            quotedText += c;

                            //для отладки
                            rawText += c;
                        }
                    }
                    else if (comment)
                    {
                        //Здесь игнорируем все до самого конца строки, но учитываем переводы строк внутри кавычек и апострофов.
                    }
                    else if (quote != (int)QuoteType.None)
                    {
                        //Строка текста внутри кавычек или апострофов
                        quotedText += c;

                        //для отладки
                        rawText += c;
                    }
                    else if (c == '{')
                    {
                        if (blockStarted && !comment)
                            blockCompleted = true;

                        //Записываем содержимое фигурных скобок
                        string curlyText = "";
                        curlyLevel = 1;
                        pos++;
                        int curlyQuote = (int)QuoteType.None;
                        while (pos < line.Length)
                        {
                            char c2 = line[pos];

                            if (c2 == '\'')
                            {
                                //Апостроф
                                if (curlyQuote == (int)QuoteType.None)
                                {
                                    curlyQuote = (int)QuoteType.Single;
                                }
                                else if (curlyQuote == (int)QuoteType.Single)
                                {
                                    if ((pos + 1 < line.Length) && (line[pos + 1] == '\''))
                                    {
                                        //Экранированный апостроф
                                        curlyText += c2;
                                        pos++;
                                        c2 = line[pos];
                                    }
                                    else
                                    {
                                        curlyQuote = (int)QuoteType.None;
                                    }
                                }
                            }
                            else if (c2 == '"')
                            {
                                //Кавычка
                                if (curlyQuote == (int)QuoteType.None)
                                {
                                    curlyQuote = (int)QuoteType.Double;
                                }
                                else if (curlyQuote == (int)QuoteType.Double)
                                {
                                    if ((pos + 1 < line.Length) && (line[pos + 1] == '"'))
                                    {
                                        //Экранированная кавычка
                                        curlyText += c2;
                                        pos++;
                                        c2 = line[pos];
                                    }
                                    else
                                    {
                                        curlyQuote = (int)QuoteType.None;
                                    }
                                }
                            }
                            else if (curlyQuote == (int)QuoteType.None)
                            {
                                if (c2 == '{')
                                {
                                    curlyLevel++;
                                }
                                else if (c2 == '}')
                                {
                                    curlyLevel--;
                                    if (curlyLevel == 0)
                                        break;
                                }
                            }
                            curlyText += c2;
                            pos++;
                        }
                        if (curlyLevel > 0)
                        {
                            SubmitError("Internal error of the analyzer - unclosed curly bracket \"{\"", currentLine);
                            break;
                        }

                        if (curlyParsing)
                        {
                            //Парсим содержимое фигурных скобок
                            List<string> curlyLines = new List<string>(curlyText.Split('\n'));
                            currentRoot = false;
                            ParseLocation(curlyLines, currentRoot);
                            currentRoot = root;
                            //Добавляем пустую "строку в кавычках", чтобы анализатор не ругался на отсутствие параметра
                            quotedText = "";
                        }
                        else
                        {
                            quotedText = curlyText;
                        }
                        quotedTextCompleted = true;
                    }
                    else
                    {
                        //Обычные символы
                        if (label)
                        {
                            if (c == '&')
                            {
                                if (labelText.Trim(WhiteSpace).Equals(":"))
                                {
                                    SubmitError("Empty label", lineNum);
                                    return false;
                                }
                                if (quote != (int)QuoteType.None)
                                {
                                    SubmitError("Unclosed string in the label name", lineNum);
                                    return false;
                                }
                                label = false;
                            }
                            else
                            {
                                labelText += c;
                            }
                        }
                        else if ((c == ':') && ((controlBlock == (int)ControlBlockType.IfCondition) ||
                                           (controlBlock == (int)ControlBlockType.ElseIfCondition)))
                        {
                            //Разбор условия
                            if (blockStarted)
                                blockCompleted = true;
                            blockGroupCompleted = true;

                            //Двоеточие после условия
                            if ((pos == line.Length - 1))
                            {
                                //В конце строки - многострочный IF или ELSEIF
                                if (firstSymbolPos == lastIfPos)
                                {
                                    if (controlBlock == (int)ControlBlockType.IfCondition)
                                    {
                                        ControlLine cline;
                                        cline.line = lineNum;
                                        cline.controlBlock = ControlBlockType.ML_If;
                                        ControlStack.Push(cline);
                                    }
                                    else
                                    {
                                        ControlLine cline = ControlStack.Pop();
                                        if (cline.controlBlock != ControlBlockType.ML_If)
                                        {
                                            SubmitError("ELSEIF without IF", lineNum);
                                            return false;
                                        }
                                        cline.line = lineNum;
                                        ControlStack.Push(cline);
                                    }
                                    controlBlock = (int)ControlBlockType.ML_If;
                                }
                                else
                                {
                                    if (controlBlock == (int)ControlBlockType.IfCondition)
                                        SubmitError("Multiline IF should be placed at the beginning of the line", lineNum);
                                    else
                                        SubmitError("Multiline ELSEIF should be placed at the beginning of the line", lineNum);
                                    return false;
                                }
                            }
                            else if (controlBlock == (int)ControlBlockType.IfCondition)
                            {
                                //Строка не заканчивается - однострочный IF
                                if (controlBlock == (int)ControlBlockType.IfCondition)
                                {
                                    ControlLine cline;
                                    cline.line = lineNum;
                                    cline.controlBlock = ControlBlockType.SL_If;
                                    ControlStack.Push(cline);

                                    controlBlock = (int)cline.controlBlock;
                                    groupInnerCounter = 0;
                                }
                                else
                                {
                                    SubmitError("ELSEIF cannot be used in single line IF", lineNum);
                                    return false;
                                }
                            }
                            else
                            {
                                SubmitError("An operator cannot be placed after colon\":\" in multiline ELSEIF", lineNum);
                                return false;
                            }
                        }
                        else if ((c == ':') && (controlBlock == (int)ControlBlockType.ActCondition))
                        {
                            //Разбор аргумента
                            if (blockStarted)
                                blockCompleted = true;
                            blockGroupCompleted = true;

                            //Двоеточие после ACT
                            if (pos == line.Length - 1)
                            {
                                //В конце строки - многострочный ACT
                                if (firstSymbolPos == lastActPos)
                                {
                                    controlBlock = (int)ControlBlockType.ML_Act;
                                    ControlLine cline;
                                    cline.line = lineNum;
                                    cline.controlBlock = ControlBlockType.ML_Act;
                                    ControlStack.Push(cline);
                                }
                                else
                                {
                                    SubmitError("Multiline ACT should be placed at the beginning of the line", lineNum);
                                    return false;
                                }
                            }
                            else
                            {
                                //Строка не заканчивается - однострочный ACT
                                controlBlock = (int)ControlBlockType.SL_Act;
                                groupInnerCounter = 0;
                            }
                        }
                        else if (!comment && (c == '!') && !blockGroupStarted && !blockStarted)
                        {
                            //Начало комментария
                            comment = true;
                        }
                        else if ((c == ':') && (firstSymbolPos == pos))
                        {
                            //Начало метки
                            label = true;
                        }
                        else if (c.ToString().IndexOfAny(Delimiters) == 0)
                        {
                            if ((c == '&') && (bracketLevel == 0))
                            {
                                if (controlBlock == (int)ControlBlockType.IfCondition)
                                {
                                    //В условии IF недопустимо использование "&" вне скобок
                                    SubmitError("Redundant \"&\": perhaps should be \"AND\"", lineNum);
                                    return false;
                                }
                                else if (controlBlock == (int)ControlBlockType.ActCondition)
                                {
                                    //В названии ACT недопустимо использование "&" вне скобок
                                    SubmitError("Redundant \"&\"", lineNum);
                                    return false;
                                }
                                else
                                {
                                    if (blockStarted)
                                        blockCompleted = true;
                                    if (blockGroupStarted)
                                        blockGroupCompleted = true;
                                    if (blockStarted && !blockGroupStarted)
                                        singleBlockGroup = true;
                                }
                            }
                            else if (blockStarted && (c == ' ') && StartOfMultiWordOperator(block))
                            {
                                //Составной оператор - DEL ACT, DEL OBJ
                                block += c;
                            }
                            else if (blockStarted && (c == ' ') && (qspGetStatCode(block) == (int)QspStatementType.Close) &&
                                     (line.Length > pos + 3) && line.Substring(pos + 1, 3).Equals("ALL", StringComparison.OrdinalIgnoreCase))
                            {
                                //CLOSE ALL
                                block += c;
                            }
                            else if (!blockStarted && (c == '*'))
                            {
                                //Возможно, *PL, *P, *NL
                                blockStarted = true;
                                blockCompleted = false;
                                block += c;
                                blockStartPos = pos;
                            }
                            else if (!blockStarted && (c == '['))
                            {
                                //Квадратная скобка должна идти вплотную к имени массива
                                SubmitError("A square bracket has to be placed just behind the name of the array, with no spaces", lineNum);
                                return false;
                            }
                            else
                            {
                                //Самостоятельные знаки
                                if (c == '(')
                                {
                                    bracketLevel++;
                                }
                                else if (c == ')')
                                {
                                    if (bracketLevel == 0)
                                    {
                                        SubmitError("Redundant bracket \")\"", lineNum);
                                        return false;
                                    }
                                    bracketLevel--;
                                }
                                bool whiteSpace = c.ToString().IndexOfAny(WhiteSpace) == 0;

                                //Особый учет для последовательностей >=, => и т.д.
                                bool comparer = pos + 1 < line.Length;
                                String cmp = "";
                                if (comparer)
                                {
                                    cmp = line.Substring(pos, 2);
                                    comparer = cmp.Equals("<=") || cmp.Equals(">=") || cmp.Equals("=>") || cmp.Equals("=<") || cmp.Equals("<>") ||
                                               cmp.Equals("+=") || cmp.Equals("-=");
                                }
                                if (comparer)
                                {
                                    //<= >= => =< <> += -=
                                    if (blockStarted)
                                    {
                                        appendDelimiter = true;
                                        delimiterToAppend = cmp;
                                    }
                                    else
                                    {
                                        blockStarted = true;
                                        blockStartPos = pos;
                                        block = cmp;
                                    }
                                    blockCompleted = true;
                                    pos++;
                                }
                                else if (blockStarted)
                                {
                                    blockCompleted = true;
                                    if (!whiteSpace)
                                    {
                                        appendDelimiter = true;
                                        delimiterToAppend += c;
                                    }
                                    else
                                    {
                                        //Обрабатываем END IF, END ACT
                                        int blockCode = qspGetStatCode(block);
                                        if (blockCode == (int)QspStatementType.End)
                                        {
                                            int nextPos = line.LastIndexOfAny(WhiteSpace, blockStartPos + block.Length) + 1;
                                            if (nextPos < line.Length)
                                            {
                                                int delimPos = line.IndexOfAny(Delimiters, nextPos);
                                                string nextWord = "";
                                                if (delimPos == INVALID_INDEX)
                                                    nextWord = line.Substring(nextPos);
                                                else
                                                    nextWord = line.Substring(nextPos, delimPos - nextPos);
                                                int nextCode = qspGetStatCode(nextWord);
                                                if ((nextCode == (int)QspStatementType.If) || (nextCode == (int)QspStatementType.Act))
                                                {
                                                    pos = nextPos + nextWord.Length - 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (!whiteSpace)
                                {
                                    blockStarted = true;
                                    blockCompleted = true;
                                    blockStartPos = pos;
                                    block += c;
                                }
                            }
                        }
                        else
                        {
                            //Просто буквы
                            if (!blockStarted)
                            {
                                blockStarted = true;
                                blockCompleted = false;
                                blockStartPos = pos;
                            }
                            block += c;
                        }
                    }
                    //Последний символ в строке
                    if (blockStarted && (pos == line.Length - 1))
                    {
                        blockCompleted = true;
                    }

                    if (blockCompleted || quotedTextCompleted)
                    {
                        if (!quotedTextCompleted && (block.Length == 0))
                        {
                            SubmitError("Internal error of analyzer! Empty block!", lineNum);
                            return false;
                        }
                        debugText += "[" + block + "] ";
                        bool skip = false;
                        int blockCode = qspGetStatCode(block);

                        if (blockCode == (int)QspStatementType.If)
                        {
                            controlBlock = (int)ControlBlockType.IfCondition;
                            debugText += "<IfCondition> ";
                            lastIfPos = blockStartPos;
                        }
                        if (blockCode == (int)QspStatementType.Act)
                        {
                            controlBlock = (int)ControlBlockType.ActCondition;
                            debugText += "<ActCondition> ";
                            lastActPos = blockStartPos;
                        }
                        if (blockCode == (int)QspStatementType.ElseIf)
                        {
                            if (controlBlock == (int)ControlBlockType.ML_If)
                            {
                                controlBlock = (int)ControlBlockType.ElseIfCondition;
                                debugText += "<ElseIfCondition> ";
                                lastIfPos = blockStartPos;
                            }
                            else
                            {
                                SubmitError("ELSEIF без IF", lineNum);
                                return false;
                            }
                        }
                        if (blockCode == (int)QspStatementType.Else)
                        {
                            if (controlBlock == (int)ControlBlockType.ML_If)
                            {
                                if (firstSymbolPos == blockStartPos)
                                {
                                    if ((blockStartPos + block.Length == line.Length) || (line.Substring(blockStartPos + block.Length).Trim(WhiteSpace).Equals(":")))
                                    {
                                        controlBlock = (int)ControlBlockType.ML_Else;
                                        
                                        ControlStack.Pop();
                                        ControlLine cline;
                                        cline.controlBlock = ControlBlockType.ML_Else;
                                        cline.line = lineNum;
                                        ControlStack.Push(cline);
                                    }
                                    else
                                    {
                                        SubmitError("An operator cannot be placed after multiline ELSE", lineNum);
                                        return false;
                                    }
                                }
                                else
                                {
                                    SubmitError("Multiline ELSE should be at the beginning of the line", lineNum);
                                    return false;
                                }
                            }
                            else
                            {
                                if (controlBlock == (int)ControlBlockType.SL_Act)
                                {
                                    if (groupInnerCounter > 0)
                                    {
                                        if ((ControlStack.Peek().controlBlock == ControlBlockType.SL_If) && (ControlStack.Peek().line == lineNum))
                                        {
                                            if (blockGroupStarted)
                                            {
                                                blockGroupCompleted = true;
                                            }
                                        }
                                        else
                                        {
                                            SubmitError("ELSE without IF", lineNum);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        SubmitError("Invalid notation of single line ACT", lineNum);
                                        return false;
                                    }
                                }
                                else if (controlBlock == (int)ControlBlockType.SL_If)
                                {
                                    if (blockGroupStarted)
                                    {
                                        blockGroupCompleted = true;
                                    }
                                    else if (groupInnerCounter == 0)
                                    {
                                        SubmitError("After single line IF has to be any operators before ELSE", lineNum);
                                        return false;
                                    }
                                }
                                else
                                {
                                    SubmitError("ELSE without IF", lineNum);
                                    return false;
                                }
                                //Это правильный ELSE
                                if (blockStartPos + block.Length < line.Length)
                                {
                                    string restLine = line.Substring(blockStartPos + block.Length);
                                    if (restLine.Trim(WhiteSpace).StartsWith(":"))
                                    {
                                        pos += restLine.IndexOf(':') + 1;
                                    }
                                }
                                ControlStack.Pop();
                                controlBlock = (int)ControlBlockType.SL_Else;
                                groupInnerCounter = 0;
                            }
                            skip = true;
                        }
                        if (blockCode == (int)QspStatementType.End)
                        {
                            if (ControlStack.Count > 0)
                            {
                                ControlStack.Pop();
                                if (ControlStack.Count > 0)
                                {
                                    ControlLine cline = ControlStack.Peek();
                                    controlBlock = (int)cline.controlBlock;
                                }
                                else
                                {
                                    controlBlock = (int)ControlBlockType.None;
                                }
                            }
                            else
                            {
                                SubmitError("Redundant END", lineNum);
                                return false;
                            }
                            skip = true;
                        }
                        if (!skip)
                        {
                            if (!blockGroupStarted)
                            {
                                blockGroupStarted = true;
                                groupInnerCounter++;
                                if (singleBlockGroup)
                                {
                                    singleBlockGroup = false;
                                    blockGroupCompleted = true;
                                }
                            }
                            if (quotedTextCompleted)
                            {
                                blockGroup.Add(quotedText);
                                blockInGroupIsText.Add(true);
                                quotedTextCompleted = false;
                            }
                            else
                            {
                                if ((blockCode == (int)QspStatementType.Unknown) && (block.Length > 1))
                                {
                                    //Обрабатываем попытки "склеить" некоторые последовательности символов
                                    string first = block.Substring(0, 1);
                                    if (first.Equals("*"))
                                    {
                                        //Если была неудачная попытка прочесть *NL, *P, и т.д., "разделяем" блок на части
                                        blockGroup.Add(first);
                                        blockInGroupIsText.Add(false);
                                        block = block.Substring(1);
                                    }
                                }
                                blockGroup.Add(block);
                                blockInGroupIsText.Add(false);
                            }
                            if (appendDelimiter)
                            {
                                blockGroup.Add(delimiterToAppend);
                                blockInGroupIsText.Add(false);
                            }
                            if (blockGroupStarted && (pos == line.Length - 1))
                            {
                                blockGroupCompleted = true;
                            }
                        }
                        block = "";
                        blockCompleted = false;
                        blockStarted = false;
                    }
                    if (blockGroupCompleted)
                    {
                        //Разбор аргументов
                        if (blockGroup.Count == 0)
                        {
                            SubmitError("Internal error of the analyzer! Empty block!", lineNum);
                            return false;
                        }
                        if (!CheckBlockGroup(blockGroup, blockInGroupIsText))
                            return false;
                        blockGroup.Clear();
                        blockInGroupIsText.Clear();
                        blockGroupStarted = false;
                        blockGroupCompleted = false;
                    }

                    pos++;
                }
                
                //Строка закончилась

                if (bracketLevel>0)
                {
                    SubmitError("Redundant bracket \"(\"", lineNum);
                    return false;
                }
                if (controlBlock == (int)ControlBlockType.IfCondition)
                {
                    SubmitError("IF without \":\"", lineNum);
                    return false;
                }
                if (controlBlock == (int)ControlBlockType.ActCondition)
                {
                    SubmitError("ACT without \":\"", lineNum);
                    return false;
                }
                if ((controlBlock == (int)ControlBlockType.SL_Act) || (controlBlock == (int)ControlBlockType.SL_If) || (controlBlock == (int)ControlBlockType.SL_Else))
                {
                    if (groupInnerCounter > 0)
                    {
                        while ((ControlStack.Count > 0) && (ControlStack.Peek().controlBlock == ControlBlockType.SL_If))
                            ControlStack.Pop();
                        if (ControlStack.Count > 0)
                        {
                            ControlLine cline = ControlStack.Peek();
                            controlBlock = (int)cline.controlBlock;
                        }
                        else
                        {
                            controlBlock = (int)ControlBlockType.None;
                        }
                    }
                    else if (controlBlock == (int)ControlBlockType.SL_Act)
                    {
                        SubmitError("Invalid notation of single line ACT", lineNum);
                        return false;
                    }
                    else if (controlBlock == (int)ControlBlockType.SL_If)
                    {
                        SubmitError("Invalid notation of single line IF", lineNum);
                        return false;
                    }
                    else if (controlBlock == (int)ControlBlockType.SL_Else)
                    {
                        SubmitError("In single line IF has to be any operators after ELSE", lineNum);
                        return false;
                    }
                }

                //для отладки
                if (line.Length > 0)
                {
                    debugText += Environment.NewLine;
                }
                
                if (currentRoot)
                {
                    //Вычисляем уровень вложенности(для утилиты форматирования кода)
                    int stackSize = ControlStack.Count;
                    int level = 0;
                    if (stackSize > 0)
                    {
                        endIndentLevel = stackSize;
                        ControlLine cline = ControlStack.Peek();
                        int b = (int)cline.controlBlock;
                        bool openingLine = cline.line == currentLine;
                        if (openingLine && ((b == (int)ControlBlockType.ML_Act) || (b == (int)ControlBlockType.ML_If)))
                        {
                            level = startIndentLevel;
                        }
                        else if (openingLine && (b == (int)ControlBlockType.ML_Else))
                        {
                            level = startIndentLevel - 1;
                        }
                        else
                        {
                            level = endIndentLevel;
                        }
                    }
                    location_codeline_level.Add(currentLine, level);
                }
            }
            //Проверка на незакрытые в конце локации многострочные IF и ACT
            if (ControlStack.Count > 0)
            {
                ControlLine cline = ControlStack.Pop();
                if ((cline.controlBlock == ControlBlockType.ML_If) || (cline.controlBlock == ControlBlockType.ML_Else))
                {
                    SubmitError("Multi line IF without END", cline.line);
                    return false;
                }
                else
                {
                    SubmitError("Multi line ACT without END", cline.line);
                    return false;
                }
            }

            AddLocationLink(m_name, true, m_start);

            return true;
        }

        bool CheckBlockGroup(List<string> blockGroup, List<bool> blockInGroupIsText)
        {
            //Разбор аргументов для операторов ACT, IF, прочих операторов и функций.
            int blockCode = qspGetStatCode(blockGroup.ToArray()[0]);
            if (blockInGroupIsText[0])
                blockCode = (int)QspStatementType.Unknown;
            bool isSetOp = blockCode == (int)QspStatementType.Set;
            if ((blockCode == (int)QspStatementType.Unknown) || isSetOp)
            {
                //Это присваивание?
                int setpos = FindInTopBracketLevel(blockGroup, blockInGroupIsText, "=");
                if (setpos == INVALID_INDEX)
                    setpos = FindInTopBracketLevel(blockGroup, blockInGroupIsText, "+=");
                if (setpos == INVALID_INDEX)
                    setpos = FindInTopBracketLevel(blockGroup, blockInGroupIsText, "-=");
                if (isSetOp && (setpos == INVALID_INDEX))
                {
                    SubmitError("Not found the char \"=\"", currentLine);
                    return false;
                }
                if (setpos != INVALID_INDEX)
                {
                    //Присваивание
                    if ((setpos == 0) || (isSetOp && (setpos == 1)))
                    {
                        SubmitError("The name of a variable before the character \"=\" is not specified!", currentLine);
                        return false;
                    }
                    int last = blockGroup.Count - 1;
                    if (setpos==last)
                    {
                        SubmitError("It should be a value after char \"=\"", currentLine);
                        return false;
                    }

                    //Проверяем левую часть
                    int start = 0;
                    if (isSetOp)
                        start = 1;
                    int count = setpos - start;
                    List<string> args = new List<string>();
                    List<bool> types = new List<bool>();
                    args = blockGroup.GetRange(start, count);
                    types = blockInGroupIsText.GetRange(start, count);
                    if (!CheckLeftAssign(args, types))
                        return false;

                    string assignedVarName = args[0];

                    //Проверяем правую часть
                    start = setpos + 1;
                    count = last - start + 1;
                    args = blockGroup.GetRange(start, count);
                    types = blockInGroupIsText.GetRange(start, count);
                    if (!CheckExpression(args, types))
                        return false;

                    //Засчитываем присваивание некоторым переменным как обращение к локации
                    if ((count == 1) && types[0] && ListContainsIgnoreCase(callerVariables, assignedVarName))
                    {
                        AddLocationLink(args[0], false, true);
                    }
                }
                else
                {
                    //Если оператор неизвестен, пытаемся разобрать выражение
                    if (!CheckExpression(blockGroup, blockInGroupIsText))
                        return false;

                    //Разбираем код в ссылках <a href="EXEC:GOTO 'loc1'">
                    if ((blockGroup.Count == 1) && (blockInGroupIsText[0]))
                    {
                        ParseExecInOutputText(blockGroup[0]);
                    }
                }
            }
            else if ((blockCode == (int)QspStatementType.If) || (blockCode == (int)QspStatementType.ElseIf))
            {
                int start = 1;
                int count = blockGroup.Count - 1;
                if (count > 0)
                {
                    List<string> args = blockGroup.GetRange(start, count);
                    List<bool> types = blockInGroupIsText.GetRange(start, count);
                    if (!CheckExpression(args, types))
                        return false;
                }
                else
                {
                    if (blockCode == (int)QspStatementType.If)
                    {
                        SubmitError("Operator IF without condition", currentLine);
                    }
                    else
                    {
                        SubmitError("Operator ELSEIF without condition", currentLine);
                    }
                    return false;
                }
            }
            else
            {
                //Если оператор известен, проверяем его аргументы.
                int count = blockGroup.Count - 1;
                List<string> args = new List<string>();
                List<bool> types = new List<bool>();
                if (count > 0)
                {
                    args = blockGroup.GetRange(1, count);
                    types = blockInGroupIsText.GetRange(1, count);
                }

                if (!CheckStatementArgs(blockCode, args, types))
                    return false;
            }

            //Проверяем подвыражения вида <<$var>> в любых строках
            for (int i = 0; i < blockGroup.Count; i++)
            {
                if (blockInGroupIsText[i])
                {
                    if (!ParseSubExpressions(blockGroup[i]))
                        return false;
                }
            }
            return true;
        }

        bool CheckStatementArgs(int code, List<string> argsSrc, List<bool> typesSrc)
        {
            int count = argsSrc.Count;
            List<string> args = new List<string>();
            List<bool> types = new List<bool>();
            if (count > 0)
            {
                if ((count > 1) && argsSrc.ToArray()[0].Equals("(") && !typesSrc.ToArray()[0] &&
                    argsSrc.ToArray()[count - 1].Equals(")") && !typesSrc.ToArray()[count - 1])
                {
                    //Игнорируем одну пару скобок по краям
                    args = argsSrc.GetRange(1, count - 2);
                    types = typesSrc.GetRange(1, count - 2);
                }
                else
                {
                    args = argsSrc.GetRange(0, count);
                    types = typesSrc.GetRange(0, count);
                }
            }
            count = args.Count;

            if ((count == 1) && types[0])
            {
                //Отмечаем в списке явные ссылки на локации
                if ((code == (int)QspStatementType.GoSub) || (code == (int)QspStatementType.GoTo) ||
                 (code == (int)QspStatementType.XGoTo))
                {
                    AddLocationLink(args[0], false, true);
                }
                //Разбираем код в ссылках <a href="EXEC:GOTO 'loc1'">
                else if ((code == (int)QspStatementType.P) || (code == (int)QspStatementType.PL) ||
                    (code == (int)QspStatementType.NL) || (code == (int)QspStatementType.MP) ||
                    (code == (int)QspStatementType.MPL) || (code == (int)QspStatementType.MNL)) 
                {
                    ParseExecInOutputText(args[0]);
                }
                //Отмечаем в списке добавленные предметы
                else if (code == (int)QspStatementType.AddObj)
                {
                    AddObj(args[0], true, false);
                }
                //Отмечаем в списке удаленные предметы
                else if (code == (int)QspStatementType.DelObj)
                {
                    AddObj(args[0], false, true);
                }
                //Отмечаем в списке добавленные действия
                else if (code == (int)QspStatementType.Act)
                {
                    AddAct(args[0], true, false);
                }
                //Отмечаем в списке удаленные действия
                else if (code == (int)QspStatementType.DelAct)
                {
                    AddAct(args[0], false, true);
                }
            }

            int minArgCount = qspStats[code].MinArgsCount;
            int maxArgCount = qspStats[code].MaxArgsCount;
            int argCount = 0;
            int curIndex = 0;
            List<string> expr = new List<string>();
            if (count > 0)
            {
                while (true)
                {
                    if (argCount == maxArgCount)
                    {
                        SubmitError("Too many arguments for the operator", currentLine);
                        return false;
                    }

                    int index = FindInTopBracketLevel(args, types, ",", curIndex);
                    if (index != INVALID_INDEX)
                    {
                        if (index > curIndex)
                        {
                            if (!CheckExpression(args.GetRange(curIndex, index - curIndex), types.GetRange(curIndex, index - curIndex)))
                                return false;
                        }
                        else
                        {
                            SubmitError("Argument can not be empty", currentLine);
                            return false;
                        }
                    }
                    else
                    {
                        if (!CheckExpression(args.GetRange(curIndex, count - curIndex), types.GetRange(curIndex, count - curIndex)))
                            return false;
                    }
                    argCount++;
                    if (index == INVALID_INDEX)
                        break;
                    curIndex = index + 1;
                    if (curIndex == count)
                    {
                        SubmitError("Argument can not be empty", currentLine);
                        return false;
                    }
                }
            }
            if (argCount < minArgCount)
            {
                SubmitError("The operator requires more arguments", currentLine);
                return false;
            }
            return true;
        }

        bool CheckExpression(List<string> args, List<bool> types)
        {
            //Проверка выражения
            int[] opStack = new int[QSP_STACKSIZE];
            int[] argStack = new int[QSP_STACKSIZE];

	        bool waitForOperator = false;
            int opCode, itemsCount = 0, opSp = -1, argSp = -1;
	        if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, (int)QspFunctionType.Start))
                return false;
            int argIndex = 0;
	        while (argIndex <= args.Count)
	        {
                String s = "";
                if (argIndex < args.Count)
                    s = args[argIndex];
		        if (waitForOperator)
		        {
			        opCode = qspGetFunctionCode(s, false);
                    String nextArg = "";
                    bool nextArgIsString = false;
                    bool hasNextArg = argIndex + 1 < args.Count;
                    if (hasNextArg)
                    {
                        nextArg = args[argIndex + 1];
                        nextArgIsString = types[argIndex + 1];
                    }
                    if (opCode == (int)QspFunctionType.Unknown || opCode >= (int)QspFunctionType.First_Function)
			        {
                        SubmitError("Unknown action in the expression", currentLine);
				        break;
			        }
                    if (((opCode == (int)QspFunctionType.And) || (opCode == (int)QspFunctionType.Or) || (opCode == (int)QspFunctionType.Mod)) &&
                        !(hasNextArg && (nextArgIsString || nextArg.Equals("(") || (nextArg.IndexOfAny(Delimiters) == INVALID_INDEX)))
                        )
			        {
                        SubmitError("Syntax error", currentLine);
				        break;
			        }
                    bool bError = false;
			        while (qspOps[opCode].Priority <= qspOps[opStack[opSp]].Priority && qspOps[opStack[opSp]].Priority != 127)
			        {
                        if (opStack[opSp] >= (int)QspFunctionType.First_Function) ++argStack[argSp];
                        if (!qspAppendToCompiled(ref itemsCount))
                        {
                            bError = true;
                            break;
                        }
				        if (--opSp < 0 || --argSp < 0)
				        {
                            SubmitError("Syntax error", currentLine);
                            bError = true;
					        break;
				        }
			        }
                    if (bError) 
                        break;
			        switch (opCode)
			        {
                    case (int)QspFunctionType.End:
				        if (opSp > 0)
				        {
                            SubmitError("Closing bracket not found", currentLine);
                            bError = true;
                            break;
				        }
				        return itemsCount > 0;
                    case (int)QspFunctionType.CloseBracket:
                        if (opStack[opSp] != (int)QspFunctionType.OpenBracket)
				        {
                            SubmitError("Opening bracket not found", currentLine);
                            bError = true;
                            break;
				        }
				        opCode = opStack[--opSp];
                        if (opCode >= (int)QspFunctionType.First_Function)
				        {
                            if (argStack[argSp] + 1 < qspOps[opCode].MinArgsCount || argStack[argSp] + 1 > qspOps[opCode].MaxArgsCount)
                            {
                                SubmitError("Invalid number of arguments", currentLine);
                                bError = true;
                            }
				        }
				        else
					        --argSp;
				        break;
                    case (int)QspFunctionType.Comma:
                        if ((opSp > 0) && opStack[opSp - 1] >= (int)QspFunctionType.First_Function)
				        {
					        if (++argStack[argSp] > qspOps[opStack[opSp - 1]].MaxArgsCount)
					        {
                                SubmitError("Invalid number of arguments", currentLine);
                                bError = true;
                                break;
					        }
				        }
				        else
				        {
                            if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, (int)QspFunctionType.Comma))
                            {
                                bError = true;
                                break;
                            }
				        }
				        waitForOperator = false;
				        break;
			        default:
                        if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, opCode))
                        {
                            bError = true;
                            break;
                        }
				        waitForOperator = false;
				        break;
			        }
			        if (bError)
                        break;
		        }
		        else
		        {
                    if ((argIndex < args.Count) && !types[argIndex] && IsNumber(s))
			        {
                        if (opStack[opSp] == (int)QspFunctionType.Minus)
				        {
					        --opSp;
					        --argSp;
				        }
				        if (!qspAppendToCompiled(ref itemsCount))
				            break;
				        waitForOperator = true;
			        }
                    else if ((argIndex < args.Count) && types[argIndex])
			        {
				        if (!qspAppendToCompiled(ref itemsCount))
				            break;
				        waitForOperator = true;
			        }
			        else if (s.Equals("+"))
			        {
			        }
			        else if (s.Equals("-"))
			        {
                        if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, (int)QspFunctionType.Minus))
                            break;
			        }
			        else if (s.Equals("("))
			        {
                        if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, (int)QspFunctionType.OpenBracket))
				            break;
			        }
			        else if (s.Equals(")"))
			        {
				        opCode = opStack[opSp];
                        if (opCode != (int)QspFunctionType.OpenBracket)
				        {
                            if (opCode >= (int)QspFunctionType.First_Function)
                                SubmitError("Invalid number of arguments", currentLine);
					        else
                                SubmitError("Syntax error", currentLine);
					        break;
				        }
				        opCode = opStack[--opSp];
                        if (opCode < (int)QspFunctionType.First_Function)
				        {
                            SubmitError("Syntax error", currentLine);
					        break;
				        }
				        if (qspOps[opCode].MinArgsCount > 0)
				        {
                            SubmitError("Invalid number of arguments", currentLine);
					        break;
				        }
				        if (!qspAppendToCompiled(ref itemsCount))
				            break;
				        --opSp;
				        --argSp;
				        waitForOperator = true;
			        }
			        else if ((argIndex < args.Count) && s.IndexOfAny(Delimiters) != 0)
			        {
                        if (s.IndexOfAny(Delimiters) != INVALID_INDEX)
                        {
                            SubmitError("Internal error of the analyzer: Invalid characters in the variable name", currentLine);
                            break;
                        }

                        opCode = qspGetFunctionCode(s, true);
                        if (opCode >= (int)QspFunctionType.First_Function)
				        {
                            if ((args.Count - argIndex > 1) && (args[argIndex + 1].Equals("(") && !types[argIndex + 1]))
					        {
						        if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, opCode))
						            break;
                                if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, (int)QspFunctionType.OpenBracket))
						            break;
                                argIndex++;
						        --argSp;
					        }
					        else if (qspOps[opCode].MinArgsCount < 2)
					        {
						        if (qspOps[opCode].MinArgsCount > 0)
						        {
							        if (!qspCompileExprPushOpCode(ref opStack, ref opSp, ref argStack, ref argSp, opCode))
							            break;
						        }
						        else
						        {
				                    if (!qspAppendToCompiled(ref itemsCount))
				                        break;
							        waitForOperator = true;
						        }
					        }
					        else
					        {
                                SubmitError("Brackets not found", currentLine);
                                break;
					        }
				        }
				        else
				        {
                            int optArgsCount = 0;

                            //Проверяем выражение(индекс массива)
                            if ((args.Count - argIndex > 1) && (args[argIndex + 1].Equals("[") && !types[argIndex + 1]))
                            {
                                int optIndex = argIndex + 2;
                                int level = 1;
                                bool bFound = false;
                                while (optIndex < args.Count)
                                {
                                    if (args[optIndex].Equals("[") && !types[optIndex])
                                    {
                                        level++;
                                    }
                                    else if (args[optIndex].Equals("]") && !types[optIndex])
                                    {
                                        level--;
                                        if (level == 0)
                                        {
                                            bFound = true;
                                            break;
                                        }
                                    }
                                    optIndex++;
                                }
                                if (level > 0)
                                {
                                    SubmitError("Unclosed parenthesis \"[\"", currentLine);
                                    break;
                                }
                                if (bFound)
                                    optArgsCount = optIndex - argIndex;
                            }
                            List<string> subArgs = args.GetRange(argIndex, optArgsCount + 1);
                            List<bool> subTypes = types.GetRange(argIndex, optArgsCount + 1);

                            if (!CheckVariable(subArgs, subTypes, false))
                                break;
                            
                            argIndex += optArgsCount;

				            if (!qspAppendToCompiled(ref itemsCount))
				                break;
					        waitForOperator = true;
				        }
			        }
			        else
			        {
                        if (opStack[opSp] >= (int)QspFunctionType.First_Function)
                            SubmitError("Invalid number of arguments", currentLine);
                        else
                            SubmitError("Syntax error", currentLine);
                        break;
			        }
		        }
                argIndex++;
	        }
            return false;
        }

        bool CheckVariable(List<string> args, List<bool> types, bool assignment)
        {
            //Проверка имени переменной
            if ((args.Count == 0) || (args[0].Length == 0) || types[0])
            {
                SubmitError("Internal error of the analyzer: empty name of variable!", currentLine);
                return false;
            }

            string name = args[0];
            int code1 = qspGetStatCode(name);
            int code2 = qspGetFunctionCode(name, true);
            if ((code1 != (int)QspStatementType.Unknown) || (code2 != (int)QspFunctionType.Unknown))
            {
                SubmitError("QSP Keyword \"" + name + "\" cannot be used as a variable name", currentLine);
                return false;
            }

            //Проверяем имя переменной на валидность
            //В имени переменной разрешены все символы, кроме DELIMS и цифры в начале имени
            //Перед именем переменной может находиться префикс "$"
            string checkName = name;
            if (checkName.StartsWith("$"))
                checkName = checkName.Substring(1);
            // $ =
            if (checkName.Length == 0)
            {
                SubmitError("Invalid variable name \"" + name + "\"", currentLine);
                return false;
            }
            // 1asd =
            if (checkName.Substring(0, 1).IndexOfAny(Digits) != INVALID_INDEX)
            {
                SubmitError("Variable name \"" + name + "\" cannot start with a digit", currentLine);
                return false;
            }
            // /asd =
            if (checkName.IndexOfAny(Delimiters) != INVALID_INDEX)
            {
                SubmitError("Invalid symbols in the name of the variable \"" + name + "\"", currentLine);
                return false;
            }

            //Проверяем переменную на "смешанные раскладки"
            if ((checkName.IndexOfAny(LatinLetters) != INVALID_INDEX) && (checkName.IndexOfAny(RussianLetters) != INVALID_INDEX))
            {
                SubmitWarning("English and Russian symbols are used simultaneously in the name of varible \"" + name + "\" , possibly a typo", currentLine);
            }

            //Проверяем выражение(индекс массива)
            if (args.Count > 1)
            {
                if (args.Count > 2 && args[1].Equals("[") && !types[1] &&
                                        args[args.Count - 1].Equals("]") && !types[args.Count - 1])
                {
                    int start = 2;
                    int count = args.Count - 3;
                    if (count > 0)
                    {
                        List<string> argsCut = args.GetRange(start, count);
                        List<bool> typesCut = types.GetRange(start, count);
                        if (!CheckExpression(argsCut, typesCut))
                            return false;
                    }
                }
                else
                {
                    SubmitError("Invalid expression", currentLine);
                    return false;
                }
            }

            //Сохраняем имя переменной в глобальном списке
            if (!assignment)
            {
                bool usedBySystem = ListContainsIgnoreCase(systemVariables, name);
                AddVar(name, usedBySystem, true);
            }

            return true;
        }
        
        bool CheckLeftAssign(List<string> args, List<bool> types)
        {
            //Проверка имени переменной(слева от знака "=" в операции присваивания)
            if ((args.Count == 0) || (args[0].Length == 0) || types[0])
            {
                //Текстовая строка
                SubmitError("Name of the variable should be before char \"=\" in the assignment operation", currentLine);
                return false;
            }

            if (!CheckVariable(args, types, true))
                return false;

            //Сохраняем имя переменной в глобальном списке
            bool usedBySystem = ListContainsIgnoreCase(systemVariables, args[0]);
            AddVar(args[0], true, usedBySystem);

            return true;
        }

        int FindInTopBracketLevel(List<string> args, List<bool> types, string what)
        {
            return FindInTopBracketLevel(args, types, what, 0);
        }

        int FindInTopBracketLevel(List<string> args, List<bool> types, string what, int startIndex)
        {
            //Ищем знак "=" или запятую вне скобок
            int bracketLevel = 0;
            for (int i = startIndex; i < args.Count; i++)
            {
                if (types[i])
                    continue;
                string block = args[i];
                if (block.Equals("("))
                {
                    bracketLevel++;
                }
                else if (block.Equals(")"))
                {
                    bracketLevel--;
                }
                else if (block.Equals(what) && (bracketLevel == 0))
                {
                    return i;
                }
            }
            return INVALID_INDEX;
        }

        void ParseExecInOutputText(string text)
        {
            //Разбираем код в ссылках <a href="EXEC:GOTO 'loc1'">
            int pos = 0;

            //Строим "карту" подвыражений.
            //Если подвыражения в строке содержат ошибки, то разбирать теги нет смысла.
            List<int> subExpMap = new List<int>();
            if (!ParseSubExpressions(text, ref subExpMap))
                return;
            int subExpSkipped = 0;

            while (pos < text.Length)
            {
                pos = text.IndexOf('<', pos);
                if (pos == INVALID_INDEX)
                    break;

                if (subExpMap.Count / 2 > subExpSkipped)
                {
                    int subExpStartPos = subExpMap[subExpSkipped * 2];
                    int subExpEndPos = subExpMap[subExpSkipped * 2 + 1];
                    if (subExpStartPos == pos)
                    {
                        //Пропускаем подвыражения вне тегов
                        subExpSkipped++;
                        pos = subExpEndPos + 1;
                        continue;
                    }
                }

                //Разбираем содержимое угловых скобок
                //Внутри HTML-тега кавычки экранируются не через дублирование, как в QSP, а через "\",
                //причем экранирование требуется только внутри окавыченной строки.
                List<string> blockGroup = new List<string>();
                List<bool> blockInGroupIsText = new List<bool>();
                int firstBlockPosition = INVALID_INDEX;

                string quotedText = "";
                string block = "";
                bool tagClosed = false;
                bool quotedTextCompleted = false;
                bool blockCompleted = false;
                pos++;
                int startPos = pos;
                if ((pos < text.Length) && (text[pos] == '/'))
                    startPos++;
                int tagQuote = (int)QuoteType.None;
                while (!tagClosed && (pos < text.Length))
                {
                    if (subExpMap.Count / 2 > subExpSkipped)
                    {
                        int subExpStartPos = subExpMap[subExpSkipped * 2];
                        int subExpEndPos = subExpMap[subExpSkipped * 2 + 1];
                        if (subExpStartPos == pos)
                        {
                            //Пропускаем подвыражения вне тегов
                            subExpSkipped++;
                            string subExpText = text.Substring(pos, subExpEndPos - pos + 1);
                            if (tagQuote == (int)QuoteType.None)
                                block += subExpText;
                            else
                                quotedText += subExpText;
                            pos = subExpEndPos + 1;
                            continue;
                        }
                    }

                    char c2 = text[pos];

                    if (tagQuote != (int)QuoteType.None)
                    {
                        //Апостроф либо кавычка
                        if (((tagQuote == (int)QuoteType.Single) && (c2 == '\'')) ||
                            ((tagQuote == (int)QuoteType.Double) && (c2 == '"')))
                        {
                            if ((pos - 1 > 0) && (text[pos - 1] == '\\'))
                            {
                                //Экранированный апостроф либо кавычка
                                if (!quotedTextCompleted)
                                    quotedText = quotedText.Substring(0, quotedText.Length - 1) + c2;
                            }
                            else
                            {
                                tagQuote = (int)QuoteType.None;
                                quotedTextCompleted = true;
                            }
                        }
                        else if (!quotedTextCompleted)
                        {
                            quotedText += c2;
                        }
                    }
                    else if (c2 == '>')
                    {
                        tagClosed = true;
                    }
                    else if ((block.Length == 0) && (c2 == '\''))
                    {
                        tagQuote = (int)QuoteType.Single;
                    }
                    else if ((block.Length == 0) && (c2 == '"'))
                    {
                        tagQuote = (int)QuoteType.Double;
                    }
                    else if (c2.ToString().IndexOfAny(WhiteSpace) == 0)
                    {
                        if (block.Length > 0)
                        {
                            blockCompleted = true;
                        }
                    }
                    else if (c2 == '=')
                    {
                        if (block.Length > 0)
                        {
                            blockGroup.Add(block);
                            blockInGroupIsText.Add(false);
                        }
                        block = "=";
                        blockCompleted = true;
                    }
                    else
                    {
                        if (firstBlockPosition == INVALID_INDEX)
                        {
                            firstBlockPosition = pos - startPos;
                        }
                        block += c2;
                    }
                    if (tagClosed)
                    {
                        if (block.Length > 0)
                            blockCompleted = true;
                    }

                    if (blockCompleted)
                    {
                        blockGroup.Add(block);
                        blockInGroupIsText.Add(false);
                        blockCompleted = false;
                        block = "";
                    }
                    if (quotedTextCompleted)
                    {
                        blockGroup.Add(quotedText);
                        blockInGroupIsText.Add(true);
                        quotedTextCompleted = false;
                        quotedText = "";
                    }

                    pos++;
                }
                
                if (tagQuote == (int)QuoteType.Single)
                {
                    SubmitWarning("Unclosed apostrophe inside the tag", currentLine);
                    break;
                }
                else if (tagQuote == (int)QuoteType.Double)
                {
                    SubmitWarning("Unclosed quote inside the tag", currentLine);
                    break;
                }
                if (!tagClosed)
                {
                    SubmitWarning("Unclosed tag",currentLine);
                    break;
                }
                if (firstBlockPosition > 0)
                {
                    SubmitWarning("The name of the tag should follow immediately after the opening angle bracket, without spaces", currentLine);
                    break;
                }


                //Ищем HREF в атрибутах тега A
                int hrefIndex = INVALID_INDEX;
                if ((blockGroup.Count > 3) && (blockGroup[0].Equals("A", StringComparison.OrdinalIgnoreCase)) && !blockInGroupIsText[0])
                {
                    for (int i = 1; i + 2 < blockGroup.Count; i++)
                    {
                        if (blockGroup[i].Equals("HREF", StringComparison.OrdinalIgnoreCase) && !blockInGroupIsText[i] &&
                            blockGroup[i + 1].Equals("=") && !blockInGroupIsText[i + 1])
                        {
                            hrefIndex = i + 2;
                        }
                    }
                }

                if (hrefIndex != INVALID_INDEX)
                {
                    string hrefText = blockGroup[hrefIndex];
                    int execKeywordLength = "EXEC:".Length;
                    if (hrefText.StartsWith("EXEC:", StringComparison.OrdinalIgnoreCase) && (hrefText.Length > execKeywordLength))
                    {
                        string execText = hrefText.Substring(execKeywordLength);
                        List<int> subExpMapExec = new List<int>();
                        ParseSubExpressions(execText, ref subExpMapExec);
                        if (subExpMapExec.Count == 0)
                        {
                            //В EXEC не разбираем код с подвыражениями, это невозможно без интерпретации
                            List<string> tagLines = new List<string>(execText.Split('\n'));
                            bool wasRoot = currentRoot;
                            currentRoot = false;
                            ParseLocation(tagLines, currentRoot);
                            currentRoot = wasRoot;
                        }
                    }
                }
            }
        }

        bool ParseSubExpressions(string text)
        {
            List<int> dummy = null;
            return ParseSubExpressions(text, ref dummy);
        }
        bool ParseSubExpressions(string text, ref List<int> subExpMap)
        {
            bool quiet = subExpMap != null;
            //Разбираем подвыражения - <<$var>>
            if (quiet)
                subExpMap.Clear();
            int pos = 0;
            bool valid = true;
            while (valid && pos < text.Length)
            {
                pos = text.IndexOf("<<", pos);
                if (pos == INVALID_INDEX)
                    break;
                if (quiet)
                    subExpMap.Add(pos);
                //Разбираем содержимое двойных угловых скобок
                string subText = "";
                bool subClosed = false;
                pos += 2;
                int subQuote = (int)QuoteType.None;
                int curlyLevel = 0;
                while (pos < text.Length)
                {
                    char c2 = text[pos];

                    if (subQuote == (int)QuoteType.None)
                    {
                        if ((c2 == '>') && (pos + 1 < text.Length) && (text[pos + 1] == '>') && (curlyLevel == 0))
                        {
                            subClosed = true;
                            break;
                        }
                        else if (c2 == '\'')
                        {
                            subQuote = (int)QuoteType.Single;
                        }
                        else if (c2 == '"')
                        {
                            subQuote = (int)QuoteType.Double;
                        }
                        else if (c2 == '{')
                        {
                            curlyLevel++;
                        }
                        else if (c2 == '}')
                        {
                            curlyLevel--;
                        }
                    }
                    else if (((subQuote == (int)QuoteType.Single) && (c2 == '\'')) ||
                             ((subQuote == (int)QuoteType.Double) && (c2 == '"')))

                    {
                        //Апостроф или кавычка
                        if ((pos + 1 < text.Length) && (
                            ((subQuote == (int)QuoteType.Single) && (text[pos + 1] == '\'')) ||
                            ((subQuote == (int)QuoteType.Double) && (text[pos + 1] == '"'))))
                        {
                            //Экранированный апостроф либо кавычка
                        }
                        else
                        {
                            subQuote = (int)QuoteType.None;
                        }
                    }
                    subText += c2;
                    pos++;
                }
                if (quiet)
                {
                    if (subClosed)
                        subExpMap.Add(pos + 1);
                    else
                        subExpMap.Add(pos - 1);
                }
                if (subQuote == (int)QuoteType.Single)
                {
                    if (!quiet)
                        SubmitError("Unclosed apostrophe in a subexpression", currentLine);
                    valid = false;
                    break;
                }
                else if (subQuote == (int)QuoteType.Double)
                {
                    if (!quiet)
                        SubmitError("Unclosed quote in a subexpression", currentLine);
                    valid = false;
                    break;
                }
                else if (curlyLevel != 0)
                {
                    if (!quiet)
                        SubmitError("Unclosed curly bracket in a subexpression", currentLine);
                    valid = false;
                    break;
                }
                if (!subClosed)
                {
                    if (!quiet)
                        SubmitError("Unclosed subexpression", currentLine);
                    valid = false;
                    break;
                }
                if (subText.Trim(WhiteSpace).Length == 0)
                {
                    if (!quiet)
                        SubmitError("Empty subexpression", currentLine);
                    valid = false;
                    break;
                }

                if (!quiet)
                {
                    //Парсим подвыражение как исполняемый код.
                    //Для того, чтобы строка считалась именно выражением, добавляем ''+ в начало
                    subText = "''+" + subText;
                    List<string> subLines = new List<string>(subText.Split('\n'));
                    bool wasRoot = currentRoot;
                    currentRoot = false;
                    valid = ParseLocation(subLines, currentRoot);
                    currentRoot = wasRoot;
                }
            }
            return valid;
        }
    }

    public class QSPGameCode : Common
    {
        Hashtable m_LocationsByName;
        List<Location> m_LocationsByOrder;
        Location m_lastLocation;

        public QSPGameCode()
        {
            m_LocationsByName = new Hashtable();
            m_LocationsByOrder = new List<Location>();
            m_lastLocation = null;
        }

        public bool ParseGame(string fileName, object sender, DoWorkEventArgs e)
        {
            int totalLines = 0;
            StreamReader rTl = new StreamReader(fileName);
            while (rTl.ReadLine() != null) { totalLines++; };
            m_LocationsByName.Clear();
            m_LocationsByOrder.Clear();
            ClearErrors();
            m_lastLocation = null;
            currentLocation = "";

            vars = new List<QspVariable>();
            locationLinks = new List<QspLocationLink>();
            objects = new List<QspObj>();
            acts = new List<QspAct>();

            StreamReader fi = null;
            if (!OpenStreamForReading(ref fi, fileName))
            {
                return false;
            }
            string s;
            int line_counter = 0;
            List<string> locationCode = new List<string>();
            bool inside = false;
            while ((s = fi.ReadLine()) != null)
            {
                line_counter++;
                string trimmed = s.Trim(WhiteSpace);
                if ((trimmed.Length > 0) && (trimmed[0] == '#'))
                {
                    //имя локации
                    string locName = trimmed.Substring(1).Trim(WhiteSpace);
                    if (!AddLocation(locName, line_counter))
                    {
                        fi.Close();
                        return false;
                    }
                    currentLocation = locName;
                    inside = true;
                }
                else if ((trimmed.Length > 38) && trimmed.StartsWith("--- ") && trimmed.EndsWith(" ---------------------------------"))
                {
                    inside = false;
                    Location lastLoc = GetLastLocation();
                    if (lastLoc != null)
                    {
                        lastLoc.ParseLocation(locationCode, true);
                        locationCode.Clear();
                        currentLocation = "";
                    }
                }
                else
                {
                    if (inside && GetLastLocation() != null)
                        locationCode.Add(s);
                }
                (sender as BackgroundWorker).ReportProgress(Convert.ToInt32(((double)line_counter/(double)totalLines) * 100));
                if ((sender as BackgroundWorker).CancellationPending)
                {
                    e.Cancel = true;
                    return true;
                }
            }
            fi.Close();
            return true;
        }

        public bool AddLocation(string name, int line)
        {
            if (name.Length <= 0)
            {
                SubmitError("Empty name of location", line);
                return false;
            }
            if (m_LocationsByName.ContainsKey(name))
            {
                SubmitError("Location named " + name + " exist already", line);
                return false;
            }
            Location newLoc = new Location(this, name, line, m_LocationsByOrder.Count == 0);
            m_LocationsByName.Add(name, newLoc);
            m_LocationsByOrder.Add(newLoc);
            m_lastLocation = newLoc;
            return true;
        }

        public Location GetLastLocation()
        {
            return m_lastLocation;
        }

        public Hashtable GetLocationsByName()
        {
            return m_LocationsByName;
        }

        public Location GetLocation(string locationName)
        {
            Location loc = (Location)m_LocationsByName[locationName];
            return loc;
        }

        public bool Beautify(string fileName)
        {
            //Причесываем код - добавляем пробелов
            StreamWriter fo = null;
            if (!OpenStreamForWriting(ref fo, fileName))
                return false;

            for (int i = 0; i < m_LocationsByOrder.Count; i++)
            {
                Location loc = (Location)m_LocationsByOrder[i];
                fo.WriteLine("# " + loc.GetName());
                for (int j = 0; j < loc.GetCodeLinesCount(); j++)
                {
                    string text = "";
                    int level = INVALID_INDEX;
                    loc.GetCodeLine(j, ref text, ref level);
                    if (level != INVALID_INDEX)
                    {
                        text = new string(' ', level * 4) + text.TrimStart(WhiteSpace);
                    }
                    fo.WriteLine(text);
                }
                fo.WriteLine("--- " + loc.GetName() + " ---------------------------------");
                fo.WriteLine();
            }
            fo.Close();
            return true;
        }
    }
}