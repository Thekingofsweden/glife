using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Analyser
{
    public class Common
    {
        const int QSP_MAXSTATSNAMES = 100;
        const int QSP_STATSLEVELS = 3;
        const int QSP_STATMAXARGS = 10;
	    const int QSP_OPSLEVELS = 2;
	    const int QSP_MAXOPSNAMES = 100;
        const int QSP_OPMAXARGS = 10;
	    public const int QSP_STACKSIZE = 30;
        public const int QSP_MAXITEMS = 100;

        public const int INVALID_INDEX = -1;
        static public char[] WhiteSpace = {' ', '\t', '\r', '\n'};
        //Разделители - " \t&'\"()[]=!<>+-/*:,{}"
        static public char[] Delimiters = " \t&'\"()[]=!<>+-/*:,{}".ToCharArray();
        static public char[] Digits = "0123456789".ToCharArray();
        static public char[] LatinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqsrtuvwxyz".ToCharArray();
        static public char[] RussianLetters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъьэюя".ToCharArray();
        public struct QspStatement
        {
            public int ExtArg;
            public int MinArgsCount;
            public int MaxArgsCount;
            public int[] ArgsTypes;
        }
        public struct QspStatementName
        {
            public string Name;
            public int NameLen;
            public int Code;
        }
        public enum QspStatementType
        {
            Unknown,
            Label,
            Comment,
            Act,
            For,
            Local,
            If,
            ElseIf,
            Else,
            End,
            AddObj,
            ClA,
            Clear,
            CloseAll,
            Close,
            ClS,
            CmdClear,
            CopyArr,
            DelAct,
            DelObj,
            Dynamic,
            Exec,
            Exit,
			KillQST,
			DelLib,
			FreeLib,
            GoSub,
            GoTo,
            AddQST,
			AddLib,
			IncLib,
			Jump,
            KillAll,
            KillObj,
            KillVar,
            MClear,
            Menu,
            MNL,
            MPL,
            MP,
            Msg,
            NL,
            OpenGame,
            OpenQst,
            Play,
            PL,
            P,
            RefInt,
            SaveGame,
            SetTimer,
            Set,
            ShowActs,
            ShowInput,
            ShowObjs,
            ShowVars,
            UnSelect,
            View,
            Wait,
            XGoTo,

            Last_Statement
        };

        public struct QspFunction
        {
		    public int Priority;
		    public int ResType;
		    public int MinArgsCount;
		    public int MaxArgsCount;
		    public int[] ArgsTypes;
        }
        public struct QspFunctionName
        {
            public string Name;
            public int NameLen;
            public int Code;
        }
        public enum QspFunctionType
        {
            Unknown,
            Start,
            End,
            Value,
            OpenBracket,
            Minus,
            Comma,
            CloseBracket,
            Mul,
            Div,
            Add,
            Sub,
            Mod,
            Ne,
            Leq,
            Geq,
            Eq,
            Lt,
            Gt,
            And,
            Or,
            Append,

            First_Function,
            Not = First_Function,
            Loc,
            Obj,
            Min,
            Max,
            Rand,
            IIf,
            RGB,
            Len,
            IsNum,
            LCase,
            UCase,
            Input,
            Str,
            Val,
            ArrSize,
            IsPlay,
            Desc,
            Trim,
            GetObj,
            StrComp,
            StrFind,
            StrPos,
            Mid,
            ArrPos,
            ArrComp,
            Instr,
            Replace,
            Func,
            DynEval,
            Rnd,
            CountObj,
            MsecsCount,
            QSPVer,
            UserText,
            CurLoc,
            SelObj,
            SelAct,
            MainText,
            StatText,
            CurActs,

            Last_Operation
        };

        static public QspStatement[] qspStats;
        static QspStatementName[,] qspStatsNames;
        static int[] qspStatsNamesCounts;
        static int qspStatMaxLen;
        static public QspFunction[] qspOps;
        static QspFunctionName[,] qspOpsNames;
        static int[] qspOpsNamesCounts;
        static int qspOpMaxLen;
        static bool inited = false;

        public class QspVariable
        {
            public bool IsString;
            public string Name;
            public bool Assigned;
            public bool Used;
            public QspVariable(string name, bool assigned, bool used)
            {
                IsString = name.IndexOf('$') == 0;
                Name = name;
                Assigned = assigned;
                Used = used;
            }
        }
        public class QspLocationLink
        {
            public string LocationName;
            public bool LocationExists;
            public bool LocationCalled;
            public QspLocationLink(string name, bool exists, bool called)
            {
                LocationName = name;
                LocationExists = exists;
                LocationCalled = called;
            }
        }
        public class QspObj
        {
            public string Name;
            public bool Added;
            public bool Removed;
            public QspObj(string name, bool added, bool removed)
            {
                Name = name;
                Added = added;
                Removed = removed;
            }
        }
        public class QspAct
        {
            public string Name;
            public bool Added;
            public bool Removed;
            public QspAct(string name, bool added, bool removed)
            {
                Name = name;
                Added = added;
                Removed = removed;
            }
        }
        static public List<QspVariable> vars;
        static public List<QspLocationLink> locationLinks;
        static public List<QspObj> objects;
        static public List<QspAct> acts;

        static public List<string> callerVariables;
        static public List<string> systemVariables;
        static public bool curlyParsing;

        static public string currentLocation;

        public enum QuoteType
        {
            None,
            Single,
            Double
        }

        public struct QspError
        {
            public string locationName; //Локация, на которой встретилась ошибка
            public string text;         //Текст сообщения об ошибке
            public int line;            //Номер строки в локации
            public bool isError;        //Тип ошибки, если "true" - ошибка синтаксиса, если "false" - предупреждение
        }
        static List<QspError> m_errors;
        static int m_errorCounter;
        public static int GetErrorsCount()
        {
            return m_errorCounter;
        }
        static int m_warningCounter;
        public static int GetWarningsCount()
        {
            return m_warningCounter;
        }

        public Common()
        {
            if (!inited)
            {
                inited = true;
                qspStats = new QspStatement[(int)QspStatementType.Last_Statement];
                qspStatsNames = new QspStatementName[QSP_STATSLEVELS, QSP_MAXSTATSNAMES];
                qspStatsNamesCounts = new int[QSP_STATSLEVELS];
                qspOps = new QspFunction[(int)QspFunctionType.Last_Operation];
                qspOpsNames = new QspFunctionName[QSP_OPSLEVELS, QSP_MAXOPSNAMES];
                qspOpsNamesCounts = new int[QSP_OPSLEVELS];

                m_errors = new List<QspError>();
                InitQsp();
                ClearErrors();
            }
        }

        static public void ClearErrors()
        {
            m_errors.Clear();
            m_errorCounter = 0;
            m_warningCounter = 0;
        }

        static public void ClearNonGameErrors()
        {
            for (int index = 0; index < m_errors.Count; ++index)
            {
                if (m_errors[index].isError && (m_errors[index].line == INVALID_INDEX))
                {

                    m_errors.RemoveAt(index);
                    --index;
                    m_errorCounter--;
                }
            }
        }

        static public void SubmitError(string text, int line)
        {
            QspError e;
            e.locationName = currentLocation;
            e.text = text;
            e.line = line;
            e.isError = true;
            m_errors.Add(e);
            m_errorCounter++;
        }

        static public void SubmitWarning(string text, int line)
        {
            QspError w;
            w.locationName = currentLocation;
            w.text = text;
            w.line = line;
            w.isError = false;
            m_errors.Add(w);
            m_warningCounter++;
        }

        static public List<QspError> GetErrors()
        {
            return m_errors;
        }

        static public bool StartOfMultiWordOperator(string op)
        {
            return op.ToUpper().Equals("ADD") || op.ToUpper().Equals("DEL");
        }

        static public void qspAddStatement(int statCode, int extArg, /*QSP_STATEMENT func,*/ int minArgs, int maxArgs, params int[] marker)
        {
            int i;
            qspStats[statCode].ExtArg = extArg;
            //qspStats[statCode].Func = func;
            qspStats[statCode].MinArgsCount = minArgs;
            qspStats[statCode].MaxArgsCount = maxArgs;
            qspStats[statCode].ArgsTypes = new int[QSP_STATMAXARGS];
            if (maxArgs > 0)
            {
                for (i = 0; i < maxArgs; ++i)
                    qspStats[statCode].ArgsTypes[i] = marker[i];
            }
        }

        static public void qspAddStatName(int statCode, string statName, int level)
        {
            int len = statName.Length;
            int count = qspStatsNamesCounts[level];
            qspStatsNames[level, count].Name = statName;
            qspStatsNames[level, count].NameLen = len;
            qspStatsNames[level, count].Code = statCode;
            qspStatsNamesCounts[level] = count + 1;
            /* Max length */
            if (len > qspStatMaxLen) qspStatMaxLen = len;
        }

        static public void qspAddOperation(int opCode, int priority, /*QSP_FUNCTION func,*/ int resType, int minArgs, int maxArgs, params int[] marker)
        {
	        int i;
	        qspOps[opCode].Priority = priority;
	        //qspOps[opCode].Func = func;
	        qspOps[opCode].ResType = resType;
	        qspOps[opCode].MinArgsCount = minArgs;
	        qspOps[opCode].MaxArgsCount = maxArgs;
            qspOps[opCode].ArgsTypes = new int[QSP_OPMAXARGS];
            if (maxArgs > 0)
	        {
		        for (i = 0; i < maxArgs; ++i)
			        qspOps[opCode].ArgsTypes[i] = marker[i];
	        }
        }

        static public void qspAddOpName(int opCode, string opName, int level)
        {
            int len = opName.Length;
	        int count = qspOpsNamesCounts[level];
	        qspOpsNames[level, count].Name = opName;
	        qspOpsNames[level, count].NameLen = len;
	        qspOpsNames[level, count].Code = opCode;
	        qspOpsNamesCounts[level] = count + 1;
	        /* Max length */
	        if (len > qspOpMaxLen) qspOpMaxLen = len;
        }


        static public void InitQsp()
        {
            qspStatMaxLen = 0;
            // код оператора, ExtArg, минимальное кол-во аргументов, максимальное кол-во аргументов, типы аргументов
            //                                                                                       0 - авто
            //                                                                                       1 - строка
            //                                                                                       2 - числовой
            qspAddStatement((int)QspStatementType.Else, 0, 0, 0);
            qspAddStatement((int)QspStatementType.ElseIf, 0, 1, 1, 2);
            qspAddStatement((int)QspStatementType.End, 0, 0, 0);
            qspAddStatement((int)QspStatementType.Local, 0, 0, 0);
            qspAddStatement((int)QspStatementType.Set, 0, 0, 0);
            qspAddStatement((int)QspStatementType.If, 0, 1, 1, 2);
            qspAddStatement((int)QspStatementType.Act, 0, 1, 2, 1, 1);
            qspAddStatement((int)QspStatementType.For, 0, 0, 0);
            qspAddStatement((int)QspStatementType.AddObj, 0, 1, 3, 1, 1, 2);
            qspAddStatement((int)QspStatementType.ClA, 3, 0, 0);
            qspAddStatement((int)QspStatementType.CloseAll, 1, 0, 0);
            qspAddStatement((int)QspStatementType.Close, 0, 0, 1, 1);
            qspAddStatement((int)QspStatementType.ClS, 4, 0, 0);
            qspAddStatement((int)QspStatementType.CmdClear, 2, 0, 0);
            qspAddStatement((int)QspStatementType.CopyArr, 0, 2, 4, 1, 1, 2, 2);
            qspAddStatement((int)QspStatementType.DelAct, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.DelObj, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.Dynamic, 0, 1, 10, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            qspAddStatement((int)QspStatementType.Exec, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.Exit, 0, 0, 0);
			qspAddStatement((int)QspStatementType.KillQST, 6, 0, 0);
			qspAddStatement((int)QspStatementType.DelLib, 6, 0, 0);
			qspAddStatement((int)QspStatementType.FreeLib, 6, 0, 0);
            qspAddStatement((int)QspStatementType.GoSub, 0, 1, 10, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            qspAddStatement((int)QspStatementType.GoTo, 1, 1, 10, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            qspAddStatement((int)QspStatementType.AddQST, 1, 1, 1, 1);
			qspAddStatement((int)QspStatementType.AddLib, 1, 1, 1, 1);
			qspAddStatement((int)QspStatementType.IncLib, 1, 1, 1, 1);
			qspAddStatement((int)QspStatementType.Jump, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.KillAll, 5, 0, 0);
            qspAddStatement((int)QspStatementType.KillObj, 1, 0, 1, 2);
            qspAddStatement((int)QspStatementType.KillVar, 0, 0, 2, 1, 2);
            qspAddStatement((int)QspStatementType.Menu, 0, 1, 3, 1, 2, 2);
            qspAddStatement((int)QspStatementType.MClear, 1, 0, 0);
            qspAddStatement((int)QspStatementType.MNL, 5, 0, 1, 1);
            qspAddStatement((int)QspStatementType.MPL, 3, 0, 1, 1);
            qspAddStatement((int)QspStatementType.MP, 1, 1, 1, 1);
            qspAddStatement((int)QspStatementType.Clear, 0, 0, 0);
            qspAddStatement((int)QspStatementType.NL, 4, 0, 1, 1);
            qspAddStatement((int)QspStatementType.PL, 2, 0, 1, 1);
            qspAddStatement((int)QspStatementType.P, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.Msg, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.OpenGame, 0, 0, 1, 1);
            qspAddStatement((int)QspStatementType.OpenQst, 0, 1, 1, 1);
            qspAddStatement((int)QspStatementType.Play, 0, 1, 2, 1, 2);
            qspAddStatement((int)QspStatementType.RefInt, 0, 0, 0);
            qspAddStatement((int)QspStatementType.SaveGame, 0, 0, 1, 1);
            qspAddStatement((int)QspStatementType.SetTimer, 0, 1, 1, 2);
            qspAddStatement((int)QspStatementType.ShowActs, 0, 1, 1, 2);
            qspAddStatement((int)QspStatementType.ShowInput, 3, 1, 1, 2);
            qspAddStatement((int)QspStatementType.ShowObjs, 1, 1, 1, 2);
            qspAddStatement((int)QspStatementType.ShowVars, 2, 1, 1, 2);
            qspAddStatement((int)QspStatementType.UnSelect, 0, 0, 0);
            qspAddStatement((int)QspStatementType.View, 0, 0, 1, 1);
            qspAddStatement((int)QspStatementType.Wait, 0, 1, 1, 2);
            qspAddStatement((int)QspStatementType.XGoTo, 0, 1, 10, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            /* Names */
            qspAddStatName((int)QspStatementType.Else, "ELSE", 2);
            qspAddStatName((int)QspStatementType.ElseIf, "ELSEIF", 1);
            qspAddStatName((int)QspStatementType.End, "END", 2);
            qspAddStatName((int)QspStatementType.Local, "LOCAL", 2);
            qspAddStatName((int)QspStatementType.Set, "SET", 2);
            qspAddStatName((int)QspStatementType.Set, "LET", 2);
            qspAddStatName((int)QspStatementType.If, "IF", 2);
            qspAddStatName((int)QspStatementType.Act, "ACT", 2);
            qspAddStatName((int)QspStatementType.For, "FOR", 2);
            qspAddStatName((int)QspStatementType.AddObj, "ADDOBJ", 2);
            qspAddStatName((int)QspStatementType.AddObj, "ADD OBJ", 2);
            qspAddStatName((int)QspStatementType.ClA, "CLA", 2);
            qspAddStatName((int)QspStatementType.CloseAll, "CLOSE ALL", 1);
            qspAddStatName((int)QspStatementType.Close, "CLOSE", 2);
            qspAddStatName((int)QspStatementType.ClS, "CLS", 2);
            qspAddStatName((int)QspStatementType.CmdClear, "CMDCLEAR", 2);
            qspAddStatName((int)QspStatementType.CmdClear, "CMDCLR", 2);
            qspAddStatName((int)QspStatementType.CopyArr, "COPYARR", 2);
            qspAddStatName((int)QspStatementType.DelAct, "DELACT", 2);
            qspAddStatName((int)QspStatementType.DelAct, "DEL ACT", 2);
            qspAddStatName((int)QspStatementType.DelObj, "DELOBJ", 2);
            qspAddStatName((int)QspStatementType.DelObj, "DEL OBJ", 2);
            qspAddStatName((int)QspStatementType.Dynamic, "DYNAMIC", 2);
            qspAddStatName((int)QspStatementType.Exec, "EXEC", 2);
            qspAddStatName((int)QspStatementType.Exit, "EXIT", 2);
			qspAddStatName((int)QspStatementType.KillQST, "KILLQST", 2);
			qspAddStatName((int)QspStatementType.DelLib, "DELLIB", 2);
			qspAddStatName((int)QspStatementType.FreeLib, "FREELIB", 2);
            qspAddStatName((int)QspStatementType.GoSub, "GOSUB", 2);
            qspAddStatName((int)QspStatementType.GoSub, "GS", 2);
            qspAddStatName((int)QspStatementType.GoTo, "GOTO", 2);
            qspAddStatName((int)QspStatementType.GoTo, "GT", 2);
			qspAddStatName((int)QspStatementType.AddQST, "ADDQST", 2);
			qspAddStatName((int)QspStatementType.AddLib, "ADDLIB", 2);
			qspAddStatName((int)QspStatementType.IncLib, "INCLIB", 2);
            qspAddStatName((int)QspStatementType.Jump, "JUMP", 2);
            qspAddStatName((int)QspStatementType.KillAll, "KILLALL", 2);
            qspAddStatName((int)QspStatementType.KillObj, "KILLOBJ", 2);
            qspAddStatName((int)QspStatementType.KillVar, "KILLVAR", 2);
            qspAddStatName((int)QspStatementType.Menu, "MENU", 2);
            qspAddStatName((int)QspStatementType.MClear, "*CLEAR", 2);
            qspAddStatName((int)QspStatementType.MClear, "*CLR", 2);
            qspAddStatName((int)QspStatementType.MNL, "*NL", 2);
            qspAddStatName((int)QspStatementType.MPL, "*PL", 1);
            qspAddStatName((int)QspStatementType.MP, "*P", 2);
            qspAddStatName((int)QspStatementType.Clear, "CLEAR", 2);
            qspAddStatName((int)QspStatementType.Clear, "CLR", 2);
            qspAddStatName((int)QspStatementType.NL, "NL", 2);
            qspAddStatName((int)QspStatementType.PL, "PL", 1);
            qspAddStatName((int)QspStatementType.P, "P", 2);
            qspAddStatName((int)QspStatementType.Msg, "MSG", 2);
            qspAddStatName((int)QspStatementType.OpenGame, "OPENGAME", 2);
            qspAddStatName((int)QspStatementType.OpenQst, "OPENQST", 2);
            qspAddStatName((int)QspStatementType.Play, "PLAY", 0);
            qspAddStatName((int)QspStatementType.RefInt, "REFINT", 2);
            qspAddStatName((int)QspStatementType.SaveGame, "SAVEGAME", 2);
            qspAddStatName((int)QspStatementType.SetTimer, "SETTIMER", 1);
            qspAddStatName((int)QspStatementType.ShowActs, "SHOWACTS", 2);
            qspAddStatName((int)QspStatementType.ShowInput, "SHOWINPUT", 2);
            qspAddStatName((int)QspStatementType.ShowObjs, "SHOWOBJS", 2);
            qspAddStatName((int)QspStatementType.ShowVars, "SHOWSTAT", 2);
            qspAddStatName((int)QspStatementType.UnSelect, "UNSELECT", 1);
            qspAddStatName((int)QspStatementType.UnSelect, "UNSEL", 2);
            qspAddStatName((int)QspStatementType.View, "VIEW", 2);
            qspAddStatName((int)QspStatementType.Wait, "WAIT", 2);
            qspAddStatName((int)QspStatementType.XGoTo, "XGOTO", 2);
            qspAddStatName((int)QspStatementType.XGoTo, "XGT", 2);

            //Математические операторы и функции
	        int i;
	        for (i = 0; i < QSP_OPSLEVELS; ++i) qspOpsNamesCounts[i] = 0;
	        qspOpMaxLen = 0;
            // код оператора, приоритет, тип результата, минимальное кол-во аргументов, максимальное кол-во аргументов, типы аргументов
            //                                                                                       0 - авто
            //                                                                                       1 - строка
            //                                                                                       2 - числовой
	        qspAddOperation((int)QspFunctionType.Value, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.Start, 127, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.End, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.OpenBracket, 127, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.CloseBracket, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.Minus, 18, 2, 1, 1, 2);
	        qspAddOperation((int)QspFunctionType.Add, 14, 0, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Sub, 14, 2, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Mul, 17, 2, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Div, 17, 2, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Mod, 16, 2, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Ne, 10, 2, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Leq, 10, 2, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Geq, 10, 2, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Eq, 10, 2, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Lt, 10, 2, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Gt, 10, 2, 2, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.Append, 12, 1, 2, 2, 1, 1);
	        qspAddOperation((int)QspFunctionType.Comma, 0, 1, 2, 2, 1, 1);
	        qspAddOperation((int)QspFunctionType.And, 7, 2, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Or, 6, 2, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Loc, 8, 2, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Obj, 8, 2, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Not, 8, 2, 1, 1, 2);
	        qspAddOperation((int)QspFunctionType.Min, 30, 0, 1, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.Max, 30, 0, 1, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.Rand, 30, 2, 1, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.IIf, 30, 0, 3, 3, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.RGB, 30, 2, 3, 4, 2, 2, 2, 2);
	        qspAddOperation((int)QspFunctionType.Len, 30, 2, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.IsNum, 30, 2, 1, 1, 0);
	        qspAddOperation((int)QspFunctionType.LCase, 30, 1, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.UCase, 30, 1, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Input, 30, 1, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Str, 30, 1, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Val, 30, 2, 1, 1, 0);
	        qspAddOperation((int)QspFunctionType.ArrSize, 30, 2, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.IsPlay, 30, 2, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Desc, 30, 1, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Trim, 30, 1, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.GetObj, 30, 1, 1, 1, 2);
	        qspAddOperation((int)QspFunctionType.StrComp, 30, 2, 2, 2, 1, 1);
	        qspAddOperation((int)QspFunctionType.StrFind, 30, 1, 2, 3, 1, 1, 2);
	        qspAddOperation((int)QspFunctionType.StrPos, 30, 2, 2, 3, 1, 1, 2);
	        qspAddOperation((int)QspFunctionType.Mid, 30, 1, 2, 3, 1, 2, 2);
	        qspAddOperation((int)QspFunctionType.ArrPos, 30, 2, 2, 3, 1, 0, 2);
	        qspAddOperation((int)QspFunctionType.ArrComp, 30, 2, 2, 3, 1, 0, 2);
	        qspAddOperation((int)QspFunctionType.Instr, 30, 2, 2, 3, 1, 1, 2);
	        qspAddOperation((int)QspFunctionType.Replace, 30, 1, 2, 3, 1, 1, 1);
	        qspAddOperation((int)QspFunctionType.Func, 30, 0, 1, 10, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.DynEval, 30, 0, 1, 10, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
	        qspAddOperation((int)QspFunctionType.Rnd, 30, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.CountObj, 30, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.MsecsCount, 30, 2, 0, 0);
	        qspAddOperation((int)QspFunctionType.QSPVer, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.UserText, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.CurLoc, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.SelObj, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.SelAct, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.MainText, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.StatText, 30, 1, 0, 0);
	        qspAddOperation((int)QspFunctionType.CurActs, 30, 1, 0, 0);
	        /* Names */
	        qspAddOpName((int)QspFunctionType.CloseBracket, ")", 1);
	        qspAddOpName((int)QspFunctionType.Add, "+", 1);
	        qspAddOpName((int)QspFunctionType.Sub, "-", 1);
	        qspAddOpName((int)QspFunctionType.Mul, "*", 1);
	        qspAddOpName((int)QspFunctionType.Div, "/", 1);
	        qspAddOpName((int)QspFunctionType.Mod, "MOD", 1);
	        qspAddOpName((int)QspFunctionType.Ne, "!", 1);
	        qspAddOpName((int)QspFunctionType.Ne, "<>", 0);
	        qspAddOpName((int)QspFunctionType.Leq, "<=", 0);
	        qspAddOpName((int)QspFunctionType.Leq, "=<", 0);
	        qspAddOpName((int)QspFunctionType.Geq, ">=", 0);
	        qspAddOpName((int)QspFunctionType.Geq, "=>", 0);
	        qspAddOpName((int)QspFunctionType.Eq, "=", 1);
	        qspAddOpName((int)QspFunctionType.Lt, "<", 1);
	        qspAddOpName((int)QspFunctionType.Gt, ">", 1);
	        qspAddOpName((int)QspFunctionType.Append, "&", 1);
	        qspAddOpName((int)QspFunctionType.Comma, ",", 1);
	        qspAddOpName((int)QspFunctionType.And, "AND", 1);
	        qspAddOpName((int)QspFunctionType.Or, "OR", 1);
	        qspAddOpName((int)QspFunctionType.Loc, "LOC", 1);
	        qspAddOpName((int)QspFunctionType.Obj, "OBJ", 1);
	        qspAddOpName((int)QspFunctionType.Not, "NO", 1);
	        qspAddOpName((int)QspFunctionType.Min, "MIN", 1);
	        qspAddOpName((int)QspFunctionType.Min, "$MIN", 1);
	        qspAddOpName((int)QspFunctionType.Max, "MAX", 1);
	        qspAddOpName((int)QspFunctionType.Max, "$MAX", 1);
	        qspAddOpName((int)QspFunctionType.Rand, "RAND", 1);
	        qspAddOpName((int)QspFunctionType.IIf, "IIF", 1);
	        qspAddOpName((int)QspFunctionType.IIf, "$IIF", 1);
	        qspAddOpName((int)QspFunctionType.RGB, "RGB", 1);
	        qspAddOpName((int)QspFunctionType.Len, "LEN", 1);
	        qspAddOpName((int)QspFunctionType.IsNum, "ISNUM", 1);
	        qspAddOpName((int)QspFunctionType.LCase, "LCASE", 1);
	        qspAddOpName((int)QspFunctionType.LCase, "$LCASE", 1);
	        qspAddOpName((int)QspFunctionType.UCase, "UCASE", 1);
	        qspAddOpName((int)QspFunctionType.UCase, "$UCASE", 1);
	        qspAddOpName((int)QspFunctionType.Input, "INPUT", 1);
	        qspAddOpName((int)QspFunctionType.Input, "$INPUT", 1);
	        qspAddOpName((int)QspFunctionType.Str, "STR", 1);
	        qspAddOpName((int)QspFunctionType.Str, "$STR", 1);
	        qspAddOpName((int)QspFunctionType.Val, "VAL", 1);
	        qspAddOpName((int)QspFunctionType.ArrSize, "ARRSIZE", 1);
	        qspAddOpName((int)QspFunctionType.IsPlay, "ISPLAY", 1);
	        qspAddOpName((int)QspFunctionType.Desc, "DESC", 1);
	        qspAddOpName((int)QspFunctionType.Desc, "$DESC", 1);
	        qspAddOpName((int)QspFunctionType.Trim, "TRIM", 1);
	        qspAddOpName((int)QspFunctionType.Trim, "$TRIM", 1);
	        qspAddOpName((int)QspFunctionType.GetObj, "GETOBJ", 1);
	        qspAddOpName((int)QspFunctionType.GetObj, "$GETOBJ", 1);
	        qspAddOpName((int)QspFunctionType.StrComp, "STRCOMP", 1);
	        qspAddOpName((int)QspFunctionType.StrFind, "STRFIND", 1);
	        qspAddOpName((int)QspFunctionType.StrFind, "$STRFIND", 1);
	        qspAddOpName((int)QspFunctionType.StrPos, "STRPOS", 1);
	        qspAddOpName((int)QspFunctionType.Mid, "MID", 1);
	        qspAddOpName((int)QspFunctionType.Mid, "$MID", 1);
	        qspAddOpName((int)QspFunctionType.ArrPos, "ARRPOS", 1);
	        qspAddOpName((int)QspFunctionType.ArrComp, "ARRCOMP", 1);
	        qspAddOpName((int)QspFunctionType.Instr, "INSTR", 1);
	        qspAddOpName((int)QspFunctionType.Replace, "REPLACE", 1);
	        qspAddOpName((int)QspFunctionType.Replace, "$REPLACE", 1);
	        qspAddOpName((int)QspFunctionType.Func, "FUNC", 1);
	        qspAddOpName((int)QspFunctionType.Func, "$FUNC", 1);
	        qspAddOpName((int)QspFunctionType.DynEval, "DYNEVAL", 1);
	        qspAddOpName((int)QspFunctionType.DynEval, "$DYNEVAL", 1);
	        qspAddOpName((int)QspFunctionType.Rnd, "RND", 1);
	        qspAddOpName((int)QspFunctionType.CountObj, "COUNTOBJ", 1);
	        qspAddOpName((int)QspFunctionType.MsecsCount, "MSECSCOUNT", 1);
	        qspAddOpName((int)QspFunctionType.QSPVer, "QSPVER", 1);
	        qspAddOpName((int)QspFunctionType.QSPVer, "$QSPVER", 1);
	        qspAddOpName((int)QspFunctionType.UserText, "USER_TEXT", 1);
	        qspAddOpName((int)QspFunctionType.UserText, "$USER_TEXT", 1);
	        qspAddOpName((int)QspFunctionType.UserText, "USRTXT", 1);
	        qspAddOpName((int)QspFunctionType.UserText, "$USRTXT", 1);
	        qspAddOpName((int)QspFunctionType.CurLoc, "CURLOC", 1);
	        qspAddOpName((int)QspFunctionType.CurLoc, "$CURLOC", 1);
	        qspAddOpName((int)QspFunctionType.SelObj, "SELOBJ", 1);
	        qspAddOpName((int)QspFunctionType.SelObj, "$SELOBJ", 1);
	        qspAddOpName((int)QspFunctionType.SelAct, "SELACT", 1);
	        qspAddOpName((int)QspFunctionType.SelAct, "$SELACT", 1);
	        qspAddOpName((int)QspFunctionType.MainText, "MAINTXT", 1);
	        qspAddOpName((int)QspFunctionType.MainText, "$MAINTXT", 1);
	        qspAddOpName((int)QspFunctionType.StatText, "STATTXT", 1);
	        qspAddOpName((int)QspFunctionType.StatText, "$STATTXT", 1);
	        qspAddOpName((int)QspFunctionType.CurActs, "CURACTS", 1);
	        qspAddOpName((int)QspFunctionType.CurActs, "$CURACTS", 1);
        }

        static public int qspGetStatCode(string s)
        {
            if (s.Length == 0)
                return (int)QspStatementType.Unknown;
            if (s[0] == ':')
                return (int)QspStatementType.Label;
            if (s[0] == '!')
                return (int)QspStatementType.Comment;
            int len;
            if (s.Length < qspStatMaxLen)
                len = s.Length;
            else
                len = qspStatMaxLen;
            string name = s.Substring(0, len).ToUpper().Trim(WhiteSpace);
	        for (int i = 0; i < QSP_STATSLEVELS; ++i)
	        {
                for (int j = 0; j < qspStatsNamesCounts[i]; j++)
                {
                    if (name.Equals(qspStatsNames[i,j].Name))
                        return qspStatsNames[i,j].Code;
                }
	        }
            return (int)QspStatementType.Unknown;
        }

        static public int qspGetFunctionCode(string s, bool functionsOnly)
        {
            if (s.Length == 0)
                return (int)QspFunctionType.End;
            int len;
            if (s.Length < qspOpMaxLen)
                len = s.Length;
            else
                len = qspOpMaxLen;
            string name = s.Substring(0, len).ToUpper().Trim(WhiteSpace);
            for (int i = 0; i < QSP_OPSLEVELS; ++i)
            {
                for (int j = 0; j < qspOpsNamesCounts[i]; j++)
                {
                    if ((name.Equals(qspOpsNames[i, j].Name)) && 
                        (!functionsOnly || (qspOpsNames[i, j].Code >= (int)QspFunctionType.First_Function)))
                        return qspOpsNames[i, j].Code;
                }
            }
            return (int)QspFunctionType.Unknown;
        }
        
        static public QspVariable GetVar(string name)
        {
            foreach (QspVariable var in vars)
            {
                if (var.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return var;
            }
            return null;
        }

        static public void AddVar(string name, bool assigned, bool used)
        {
            QspVariable var = GetVar(name);
            if (var == null)
            {
                var = new QspVariable(name, assigned, used);
                vars.Add(var);
            }
            else
            {
                if (assigned)
                    var.Assigned = assigned;
                if (used)
                    var.Used = used;
            }
        }

        static public QspLocationLink GetLocationLink(string name)
        {
            foreach (QspLocationLink loc in locationLinks)
            {
                if (loc.LocationName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return loc;
            }
            return null;
        }

        static public void AddLocationLink(string name, bool exists, bool called)
        {
            QspLocationLink loc = GetLocationLink(name);
            if (loc == null)
            {
                loc = new QspLocationLink(name, exists, called);
                locationLinks.Add(loc);
            }
            else
            {
                if (exists)
                    loc.LocationExists = exists;
                if (called)
                    loc.LocationCalled = called;
            }
        }

        static public QspObj GetObj(string name)
        {
            foreach (QspObj obj in objects)
            {
                if (obj.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return obj;
            }
            return null;
        }

        static public void AddObj(string name, bool added, bool removed)
        {
            QspObj obj = GetObj(name);
            if (obj == null)
            {
                obj = new QspObj(name, added, removed);
                objects.Add(obj);
            }
            else
            {
                if (added)
                    obj.Added = added;
                if (removed)
                    obj.Removed = removed;
            }
        }

        static public QspAct GetAct(string name)
        {
            foreach (QspAct act in acts)
            {
                if (act.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return act;
            }
            return null;
        }

        static public void AddAct(string name, bool added, bool removed)
        {
            QspAct act = GetAct(name);
            if (act == null)
            {
                act = new QspAct(name, added, removed);
                acts.Add(act);
            }
            else
            {
                if (added)
                    act.Added = added;
                if (removed)
                    act.Removed = removed;
            }
        }

        static public void SetConfig(List<string> callerVars, List<string> systemVars, bool EnableCurlyParsing)
        {
            callerVariables = callerVars;
            systemVariables = systemVars;
            curlyParsing = EnableCurlyParsing;
        }

        static public bool IsNumber(String s)
        {
            foreach (char c in s.ToCharArray())
            {
                if (Array.IndexOf(Digits, c) == INVALID_INDEX)
                    return false;
            }
            return true;
        }

        static public bool ListContainsIgnoreCase(List<string> stringList, string value)
        {
            bool contains = null != stringList.Find(delegate(string str)
            {
                return str.Equals(value, StringComparison.OrdinalIgnoreCase);
            });
            return contains;
        }

        static public string GenerateCsvName(string FileName)
        {
            string result = FileName;
            if (result.EndsWith(".txt"))
                result = result.Substring(0, result.Length - 4);
            result = result + ".csv";
            return result;
        }

        static public bool ExportToCsv(string srcFile, string delimiter)
        {
            //Выгружаем исходный код в CSV
            //4 колонки: номер+локация, оригинал, место для перевода, место для комментария

            string dstFile = GenerateCsvName(srcFile);

            StreamReader fi = null;
            if (!OpenStreamForReading(ref fi, srcFile))
            {
                return false;
            }
            StreamWriter fo = null;
            if (!OpenStreamForWriting(ref fo, dstFile))
            {
                fi.Close();
                return false;
            }
            string line;
            int line_counter = 0;
            int quoted_line_counter = 0;
            bool inside = false;
            string locName = null;

            int quote = (int)QuoteType.None;
            string quotedText = "";
            bool quotedTextCompleted = false;

            while ((line = fi.ReadLine()) != null)
            {
                line_counter++;
                line = line.Trim(WhiteSpace);
                if ((line.Length > 0) && (line[0] == '#'))
                {
                    //имя локации
                    locName = line.Substring(1).Trim(WhiteSpace);
                    inside = true;
                }
                else if ((line.Length > 38) && line.StartsWith("--- ") && line.EndsWith(" ---------------------------------"))
                {
                    inside = false;
                    locName = null;
                }
                else if (inside)
                {
                    //Разбираем только строки внутри локаций
                    int pos = 0;

                    //Разбор строки
                    while (pos < line.Length)
                    {
                        char c = line[pos];
                        if (c == '\'')
                        {
                            //Апостроф
                            if (quote == (int)QuoteType.None)
                            {
                                quote = (int)QuoteType.Single;
                                quotedText = "";
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
                                    quotedTextCompleted = true;
                                }
                            }
                            else
                            {
                                quotedText += c;
                            }
                        }
                        else if (c == '"')
                        {
                            //Кавычка
                            if (quote == (int)QuoteType.None)
                            {
                                quote = (int)QuoteType.Double;
                                quotedText = "";
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
                                    quotedTextCompleted = true;
                                }
                            }
                            else
                            {
                                quotedText += c;
                            }
                        }
                        else if (quote != (int)QuoteType.None)
                        {
                            //Строка текста внутри кавычек или апострофов
                            quotedText += c;
                        }
                        else if (c == '{')
                        {
                            //Пропускаем содержимое фигурных скобок
                            int curlyLevel = 1;
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
                                            pos++;
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
                                            pos++;
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
                                pos++;
                            }
                        }

                        if (quotedTextCompleted)
                        {
                            //Формируем строку для csv-файла
                            //4 колонки: номер+локация, оригинал, место для перевода, место для комментария
                            if (quotedText.Trim(WhiteSpace).Length > 0)
                            {
                                quoted_line_counter++;

                                string csv_line = "";
                                csv_line += CsvEscape(quoted_line_counter.ToString() + ":" + locName) + delimiter;
                                csv_line += CsvEscape(quotedText) + delimiter;
                                csv_line += CsvEscape("") + delimiter;
                                csv_line += CsvEscape("");

                                //Записываем строку в csv-файл
                                fo.WriteLine(csv_line);
                            }
                            quotedTextCompleted = false;
                        }
                        pos++;
                    }
                }
            }
            fi.Close();
            fo.Close();
            return true;
        }

        static public string CsvEscape(string src)
        {
            //Форматируем строку для записи в csv-файл
            string result = "\"" + src.Replace("\"", "\"\"") + "\"";
            return result;
        }

        static public string CsvUnEscape(string src)
        {
            //Преобразовываем строку из csv-формата в обычный текст
            string result = src;
            if (src.StartsWith("\""))
            {
                result = result.Substring(1, result.Length - 2);
                result = result.Replace("\"\"", "\"");
            }
            return result;
        }

        static public bool TranslateFromCsv(string srcFile, string suffix, bool IgnoreEmptyTranslations, string delimiter)
        {
            //Создаем перевод на основе загруженного файла и CSV
            //Проверяем соответствие номера, локации, оригинального текста, в случае несовпадения выдаем ошибку и не сохраняем файл
            //Для пустых строк заполняем текстом оригинала, либо выдаем ошибку - в зависимости от настроек.

            string dstFile = srcFile;
            if (dstFile.EndsWith(".txt"))
                dstFile = dstFile.Substring(0, dstFile.Length - 4);
            dstFile = dstFile + suffix + ".txt";

            string csvFile = GenerateCsvName(srcFile);

            StreamReader fi_src = null;
            if (!OpenStreamForReading(ref fi_src, srcFile))
            {
                return false;
            }
            StreamReader fi_csv = null;
            if (!OpenStreamForReading(ref fi_csv, csvFile))
            {
                fi_src.Close();
                return false;
            }
            StreamWriter fo = null;
            if (!OpenStreamForWriting(ref fo, dstFile))
            {
                fi_src.Close();
                fi_csv.Close();
                return false;
            }
            string line;
            string line_cut;
            int line_counter = 0;
            int quoted_line_counter = 0;
            bool inside = false;
            string locName = null;

            int quote = (int)QuoteType.None;
            string quotedText = "";
            bool quotedTextCompleted = false;

            string rawText = "";

            bool aborted = false;

            while ((line = fi_src.ReadLine()) != null)
            {
                line_counter++;
                line_cut = line.Trim(WhiteSpace);
                if ((line_cut.Length > 0) && (line_cut[0] == '#'))
                {
                    //имя локации
                    locName = line_cut.Substring(1).Trim(WhiteSpace);
                    inside = true;
                    fo.WriteLine(line);
                }
                else if ((line_cut.Length > 38) && line_cut.StartsWith("--- ") && line_cut.EndsWith(" ---------------------------------"))
                {
                    inside = false;
                    locName = null;
                    fo.WriteLine(line);
                }
                else if (inside)
                {
                    //Разбираем только строки внутри локаций
                    int pos = 0;

                    //Разбор строки
                    while (pos < line.Length)
                    {
                        char c = line[pos];
                        if (c == '\'')
                        {
                            //Апостроф
                            if (quote == (int)QuoteType.None)
                            {
                                quote = (int)QuoteType.Single;
                                quotedText = "";
                                rawText += c;
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
                                    quotedTextCompleted = true;
                                }
                            }
                            else
                            {
                                quotedText += c;
                            }
                        }
                        else if (c == '"')
                        {
                            //Кавычка
                            if (quote == (int)QuoteType.None)
                            {
                                quote = (int)QuoteType.Double;
                                quotedText = "";
                                rawText += c;
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
                                    quotedTextCompleted = true;
                                }
                            }
                            else
                            {
                                quotedText += c;
                            }
                        }
                        else if (quote != (int)QuoteType.None)
                        {
                            //Строка текста внутри кавычек или апострофов
                            quotedText += c;
                        }
                        else if (c == '{')
                        {
                            //Пропускаем содержимое фигурных скобок
                            int curlyLevel = 1;
                            rawText += c;
                            pos++;
                            int curlyQuote = (int)QuoteType.None;
                            while (pos < line.Length)
                            {
                                char c2 = line[pos];
                                rawText += c2;

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
                                            rawText += line[pos + 1];
                                            pos++;
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
                                            rawText += line[pos + 1];
                                            pos++;
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
                                pos++;
                            }
                        }
                        else
                        {
                            rawText += c;
                        }

                        if (quotedTextCompleted)
                        {
                            //Читаем строку из файла перевода
                            string translation = quotedText;

                            if (quotedText.Trim(WhiteSpace).Length > 0)
                            {
                                translation = "";
                                quoted_line_counter++;

                                bool csvLineCompleted = false;
                                string csv_line = null;
                                int valuesCount = 0;
                                string thirdValue = "";
                                int csvQuote = (int)QuoteType.None;
                                string csvText = "";
                                bool csvValueCompleted = false;

                                int csvLineIndex = INVALID_INDEX;
                                string csvLocName = "";
                                string csvOriginalLine = "";

                                while (!csvLineCompleted && ((csv_line = fi_csv.ReadLine()) != null))
                                {
                                    int csvPos = 0;

                                    //Разбор строки
                                    while (csvPos < csv_line.Length)
                                    {
                                        int newValuesCount = valuesCount;
                                        char c3 = csv_line[csvPos];
                                        if (c3 == '"')
                                        {
                                            csvText += c3;
                                            //Кавычка
                                            if (csvQuote == (int)QuoteType.None)
                                            {
                                                csvQuote = (int)QuoteType.Double;
                                            }
                                            else if (csvQuote == (int)QuoteType.Double)
                                            {
                                                if ((csvPos + 1 < csv_line.Length) && (csv_line[csvPos + 1] == '"'))
                                                {
                                                    //Экранированная кавычка
                                                    csvText += csv_line[csvPos + 1];
                                                    csvPos++;
                                                }
                                                else
                                                {
                                                    csvQuote = (int)QuoteType.None;
                                                }
                                            }
                                        }
                                        else if (csvQuote != (int)QuoteType.None)
                                        {
                                            //Строка текста внутри кавычек
                                            csvText += c3;
                                        }
                                        else if (c3.ToString() == delimiter)
                                        {
                                            newValuesCount++;
                                            csvValueCompleted = true;
                                        }
                                        else
                                        {
                                            csvText += c3;
                                        }

                                        if ((csvQuote == (int)QuoteType.None) && (csvPos == csv_line.Length - 1))
                                        {
                                            newValuesCount++;
                                            csvValueCompleted = true;
                                        }

                                        if (csvValueCompleted)
                                        {
                                            for (int iter = valuesCount + 1; iter <= newValuesCount; iter++)
                                            {
                                                csvText = CsvUnEscape(csvText.Trim(WhiteSpace));
                                                if (iter == 1)   //Первая колонка - имя локации и номер строки
                                                {
                                                    string[] csvFirst = csvText.Split(":".ToCharArray());
                                                    if (csvFirst.Length == 2)
                                                    {
                                                        csvLineIndex = Convert.ToInt32(csvFirst[0]);
                                                        csvLocName = csvFirst[1];
                                                    }
                                                }
                                                else if (iter == 2)   //Вторая колонка - оригинал
                                                {
                                                    csvOriginalLine = csvText;
                                                }
                                                else if (iter == 3)   //Третья колонка - перевод
                                                {
                                                    thirdValue = csvText;
                                                }
                                                csvText = "";
                                            }
                                            csvValueCompleted = false;
                                            valuesCount = newValuesCount;
                                        }
                                        csvPos++;
                                    }
                                    if (csvQuote == (int)QuoteType.None)
                                    {
                                        csvLineCompleted = true;
                                    }
                                    else
                                    {
                                        csvText += Environment.NewLine;
                                    }
                                }

                                if ((csv_line == null) || (csvLineCompleted && 
                                    ((csvLocName != locName) || (quoted_line_counter != csvLineIndex) || (csvOriginalLine != quotedText))))
                                {
                                    SubmitError("CSV-file does not match the original! Generate it again.", INVALID_INDEX);
                                    aborted = true;
                                    break;
                                }
                                if (!csvLineCompleted)
                                {
                                    SubmitError("CSV-file damaged! Line number: " + quoted_line_counter.ToString(), INVALID_INDEX);
                                    aborted = true;
                                    break;
                                }
                                translation = thirdValue;

                                if (translation.Length == 0)
                                {
                                    if (IgnoreEmptyTranslations)
                                    {
                                        translation = quotedText;
                                    }
                                    else
                                    {
                                        SubmitError("Line number " + quoted_line_counter + " is not converted!", INVALID_INDEX);
                                        aborted = true;
                                        break;
                                    }
                                }
                            }
                            rawText += translation;
                            rawText += c;
                            quotedTextCompleted = false;
                        }
                        pos++;
                    }
                    if (aborted)
                        break;
                    if (quote == (int)QuoteType.None)
                    {
                        //Записываем строку в файл перевода
                        fo.WriteLine(rawText);
                        rawText = "";
                    }
                }
                else
                {
                    fo.WriteLine(line);
                }
            }
            fi_src.Close();
            fi_csv.Close();
            fo.Close();
            if (aborted)
            {
                //Убить корявый перевод
                File.Delete(dstFile);
            }
            return !aborted;
        }

        static public bool OpenStreamForReading(ref StreamReader fi, string file)
        {
            bool result = false;
            try
            {
                fi = new StreamReader(file, Encoding.Default);
                result = true;
            }
            catch (Exception e)
            {
                SubmitError("File \"" + file + "\" cannot be opened for reading (it can be opened by another program).", INVALID_INDEX);
                result = false;
            }
            return result;
        }

        static public bool OpenStreamForWriting(ref StreamWriter fo, string file)
        {
            bool result = false;
            try
            {
                fo = new StreamWriter(file, false, Encoding.Default);
                result = true;
            }
            catch (Exception e)
            {
                SubmitError("File \"" + file + "\" cannot be opened for writing (it can be opened by another program).", INVALID_INDEX);
                result = false;
            }
            return result;
        }
    }
}
