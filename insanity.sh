#!/usr/bin/env bash

NOTICE="\e[1;34m" # bold light blue
WARNING="\e[1;31m" # bold red
RESET="\e[0m"

notice() {
    echo -e "${NOTICE}$1${RESET}"
}

warning() {
    echo -e "${WARNING}$1${RESET}"
}

show_help() {
    echo -e "-h, -?\t\tShow this message."
    echo -e "-u\t\tTry to update the typo database."
    echo -e "-n\t\tDisable git grep's pager."
}

_software_notfound() {
       echo -e "${WARNING}ERROR:\t$1 must be installed in order to update the database${RESET}"
       echo -e "\tOn a debian based distro, run \e[38;5;244m\`sudo apt-get install $1'${RESET}"
}

update_typo() {
    if ! command -v xmllint>/dev/null; then
        _software_notfound "libxml2-utils"
       exit 1
    fi
    if ! command -v recode>/dev/null; then
       _software_notfound "recode"
       exit 1
    fi

    curl -s  https://en.wikipedia.org/wiki/Wikipedia:Lists_of_common_misspellings/For_machines | xmllint --html --xpath '//pre/text()' - | recode html | awk -F"->" '{ print $1 }' | sed 's/\s$//' > ct.txt
    
    if cmp --silent common_typos.txt ct.txt; then
        rm ct.txt
        echo -e "No update necessary."
    else
        echo -e "The typo database has been updated. Differences:"
        diff -u common_typos.txt ct.txt
        mv ct.txt common_typos.txt
    fi
    
    exit 0
}

OPTIND=1

no_pager=0
while getopts "h?un" opt; do
    case "$opt" in
    h|\?)
        show_help
        exit 0
        ;;
    u)
        update_typo
        exit 0
        ;;
    n)
        no_pager=1
        ;;
    esac
done

#shift $((OPTIND-1))

if [ "$no_pager" -gt 0 ]; then
    GREP="git --no-pager grep -n --color"
else
    GREP="git grep -n --color"
fi

_build_negative_match() {
    output=""
    for arg in "$@"
    do
        output+=" --and --not -e \"${arg}\""
    done
}


notice 'Checking repeating words... (e.g. "voice voice")'
_build_negative_match " ha ha " " no no " " sweet sweet " " low low "\
                      " that that " " really really " " so so "\
                      " her her "
$GREP -e " \(\b[a-zA-Z][a-zA-Z]\+\) \1\b ""$output" -- locations/

notice 'Checking bad articles... (e.g. "a automatic" or "an color")'
$GREP -i -E "\Wa (a|e|i|o|u)." -- locations/ | grep -a -i -vE "\Wa (un|eu|us|ut|on|ur|in)."
echo -en "\e[38;5;34m-";for i in {35..39} {39..34}; do echo -en "\e[38;5;${i}m*-*-"; done; echo -e "\e[0m"
$GREP -i -E "\Wan (b|c|d|f|g|j|k|l|m|n|p|q|r|s|t|v|w|x|y|z)\w+" -- locations/

notice 'Checking syntax mistakes... (e.g. "<<\$var><")'
$GREP -a -E '<<\$[^>]+><' -- locations/
$GREP -a -E "^\W+\wact\W+'" -- locations/

notice 'Checking common typos... (e.g. "recieve")'
$GREP -i -a -E "\W(carress)" -- locations/

echo -en "\n\e[0;31m";for i in {0..80}; do echo -en "#"; done; echo -e "${RESET}"
warning 'The following will not be matching lines with variables ($variable['"''index''"']).'
echo -en "\e[0;31m";for i in {0..80}; do echo -en "#"; done; echo -e "${RESET}\n"
$GREP -i -a -E --not -e '^\W+(!|if|elseif)' --and --not -e '\$[a-zA-Z]+(\[|>)' --and --not -e 'teh' --and -e "\W($(cat common_typos.txt| tr '\n' '|'|sed 's/|$//'))\W" -- locations
