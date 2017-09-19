using System;
using System.Runtime.InteropServices;
using TM1V = System.IntPtr;

namespace cubewise.code
{

    static class API
    {

        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValErrorCode(TM1V hUser, TM1V hValue);
        [DllImport("tm1api.dll", EntryPoint = "TM1ValErrorString")]
        public static extern IntPtr TM1ValErrorStringPointer(TM1V hUser, TM1V hValue);
        internal static string TM1ValErrorString(TM1V hUser, TM1V vString)
        {
            IntPtr pointer = TM1ValErrorStringPointer(hUser, vString);
            return Marshal.PtrToStringAnsi(pointer);
        }
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValReal(TM1V hPool, double DoubleValue);
        [DllImport("tm1api.dll")]
        public static extern double TM1ValRealGet(TM1V hPool, TM1V hReal);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValBool(TM1V hPool, TM1V hValue);
        [DllImport("tm1api.dll")]
        public static extern bool TM1ValBoolGet(TM1V hUser, TM1V hValue);
        [DllImport("tm1api.dll")]


        public static extern TM1V TM1SystemClose(TM1V hUser);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1SystemOpen();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValPoolCreate(TM1V hUser);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValPoolDestroy(TM1V hPool);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1SystemAdminHostSet(TM1V hUser, string AdminHosts);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValType(TM1V hUser, TM1V Value);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ServerDimensions();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1DimensionElements();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ObjectName();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ObjectListHandleByNameGet(TM1V hPool, TM1V hObject, TM1V objectType, TM1V objectName);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ObjectPropertyGet(TM1V hPool, TM1V hObject, TM1V objectProperty);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1SystemServerConnect(TM1V hPool, TM1V sServer, TM1V sClient, TM1V sPassword);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1SystemServerConnectWithCAMNamespace(TM1V hPool, TM1V sServer, TM1V vCAMArgArray);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValIndex(TM1V hPool, uint ArrayIndex);
        [DllImport("tm1api.dll", EntryPoint = "TM1ValStringGet")]
        static extern IntPtr TM1ValStringGetPointer(TM1V hUser, TM1V vString);
        internal static string TM1ValStringGet(TM1V hUser, TM1V vString)
        {
            IntPtr pointer = TM1ValStringGetPointer(hUser, vString);
            return Marshal.PtrToStringAnsi(pointer);
        }
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValArrayMaxSize(TM1V hUser, TM1V hArray);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValArrayGet(TM1V hUser, TM1V hArray, uint ArrayIndex);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValIndexGet(TM1V hUser, TM1V hCode);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValArray(TM1V hPool, TM1V[] hValueArray, uint ArrayLength);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValArraySet(TM1V hArray, TM1V hArrayEntry, uint ArrayIndex);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValErrorString_VB(TM1V hUser, TM1V hObject, string ErrorMsg, uint ErrorMsgLength);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValString(TM1V hPool, string InitString, uint MaxSize);
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeArray();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeBool();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeError();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeIndex();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeObject();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeReal();
        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ValTypeString();

        [DllImport("tm1api.dll")]
        public static extern TM1V TM1ClientDisconnect(TM1V hPool, TM1V hServer, TM1V hUserName);
        [DllImport("tm1api.dll")]
        internal static extern TM1V TM1TopConnect(TM1V hPool, TM1V hServer);
        [DllImport("tm1api.dll")]
        internal static extern TM1V TM1TopGetCurrentState(TM1V hPool, TM1V hConnection);
        [DllImport("tm1api.dll")]
        internal static extern TM1V TM1TopKillTiProcess(TM1V hPool, TM1V hConnection, TM1V hThreadID);

    }
}
