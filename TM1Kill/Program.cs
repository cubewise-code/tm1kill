using System;
using System.Collections.Generic;
using TM1V = System.IntPtr;

namespace cubewise.code
{
    static class Program
    {

        public static void Main(string[] args)
        {

            TM1V hUser = TM1V.Zero;
            TM1V hPool = TM1V.Zero;
            TM1V hTop = TM1V.Zero;
            TM1V hTopPool = TM1V.Zero;

            try
            {

                string sUserName = "";
                string sPassword = "";
                string sServer = "";
                string sAdminHost = "";
                string sCAMNamespace = "";
                bool bUserName = false;
                bool bPassword = false;
                bool bServer = false;
                bool bAdminHost = false;
                bool bCAMNamespace = false;
                bool bCancel = false;
                bool bDisconnect = false;

                int prm = 0;
                foreach (string arg in args)
                {
                    string uArg = arg.ToUpper();

                    if (prm == 0)
                    {
                        if (uArg == "-USERNAME")
                        {
                            prm = 1;
                        }
                        else if (uArg == "-PASSWORD")
                        {
                            prm = 2;
                        }
                        else if (uArg == "-SERVER")
                        {
                            prm = 3;
                        }
                        else if (uArg == "-ADMINHOST")
                        {
                            prm = 4;
                        }
                        else if (uArg == "-CAMNAMESPACE")
                        {
                            prm = 5;
                        }
                        else if (uArg == "-CANCEL")
                        {
                            prm = 0;
                            bCancel = true;
                        }
                        else if (uArg == "-DISCONNECT")
                        {
                            prm = 0;
                            bDisconnect = true;
                        }
                        else
                        {
                            prm = 0;
                        }
                    }
                    else
                    {
                        if (prm == 1)
                        {
                            sUserName = arg;
                            bUserName = true;
                        }
                        else if (prm == 2)
                        {
                            sPassword = arg;
                            bPassword = true;
                        }
                        else if (prm == 3)
                        {
                            sServer = arg;
                            bServer = true;
                        }
                        else if (prm == 4)
                        {
                            sAdminHost = arg;
                            bAdminHost = true;
                        }
                        else if (prm == 5)
                        {
                            sCAMNamespace = arg;
                            bCAMNamespace = true;
                        }
                        prm = 0;
                    }
                }

                if (bUserName == false | bPassword == false | bServer == false | bAdminHost == false)
                {
                    Console.WriteLine("You must provide an -ADMINHOST, -SERVER, -USERNAME and -PASSWORD");
                    Console.WriteLine("For CAM security provide -CAMNAMESPACE");
                    Console.WriteLine("To cancel all threads add -CANCEL");
                    Console.WriteLine("To cancel disconnect all threads add -DISCONNECT");
                    return;
                }

                if (!bCancel && !bDisconnect)
                {
                    Console.WriteLine("You must either -CANCEL, - DISCONNECT or both");
                    return;
                }

                Console.WriteLine("Starting with settings:");
                Console.WriteLine(" Username: " + sUserName);
                Console.WriteLine(" Password: " + sPassword);
                Console.WriteLine(" AdminHost: " + sAdminHost);
                Console.WriteLine(" Server: " + sServer);
                Console.WriteLine(" Cancel: " + bCancel);
                Console.WriteLine(" Disconnect: " + bCancel);
                if (bCAMNamespace)
                {
                    Console.WriteLine("CAM Namespace: " + sCAMNamespace);
                }
                Console.WriteLine("");

                hUser = API.TM1SystemOpen();
                hPool = API.TM1ValPoolCreate(hUser);

                Console.WriteLine("Attempting to connect to server");
                API.TM1SystemAdminHostSet(hUser, sAdminHost);

                    TM1V hServer;
                if (bCAMNamespace)
                {
                    TM1V[] hArgs = new TM1V[3];
                    hArgs[0] = API.TM1ValString(hPool, sCAMNamespace, 0);
                    hArgs[1] = API.TM1ValString(hPool, sUserName, 0);
                    hArgs[2] = API.TM1ValString(hPool, sPassword, 0);

                    TM1V hArgArray = API.TM1ValArray(hPool, hArgs, 3);
                    API.TM1ValArraySet(hArgArray, hArgs[0], (uint)1);
                    API.TM1ValArraySet(hArgArray, hArgs[1], (uint)2);
                    API.TM1ValArraySet(hArgArray, hArgs[2], (uint)3);
                    hServer = API.TM1SystemServerConnectWithCAMNamespace(hPool, API.TM1ValString(hPool, sServer, 0), hArgArray);
                }
                else
                {
                    hServer = API.TM1SystemServerConnect(hPool, API.TM1ValString(hPool, sServer, 0), API.TM1ValString(hPool, sUserName, 0), API.TM1ValString(hPool, sPassword, 0));
                }

                if (API.TM1ValType(hUser, hServer) == API.TM1ValTypeError())
                {
                    Console.WriteLine("Error connecting to the server: " + API.TM1ValErrorString(hUser, hServer));
                    return;
                }
                Console.WriteLine("Successfully logged into server with user name: " + sUserName);

                // Connect to TM1 TOP API
                hTop = API.TM1SystemOpen();
                hTopPool = API.TM1ValPoolCreate(hTop);
                TM1V hConnection = API.TM1TopConnect(hTopPool, API.TM1ValString(hTopPool, sServer, 0));
                if (API.TM1ValType(hTop, hConnection) != API.TM1ValTypeObject())
                {
                    if (API.TM1ValType(hTop, hConnection) == API.TM1ValTypeError())
                    {
                        Console.WriteLine("Error connecting to TM1 Top: " + API.TM1ValErrorString(hTop, hConnection));
                    }
                    else
                    {
                        Console.WriteLine("Error connecting to TM1 Top, return type unknown");
                    }
                    return;
                }
                Console.WriteLine("Connected to TM1 Top: " + sServer);

                List<TopItem> items = GetState(hTop, hTopPool, hConnection);
                if (bCancel)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Cancelling threads...");
                    List<TopItem> toCancel = GetThreadsToCancel(items);
                    if (toCancel.Count > 0)
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            // Cancel the threads
                            foreach (TopItem item in toCancel)
                            {
                                bool result = CancelThread(hTop, hTopPool, hConnection, item.ThreadId);
                                if (result)
                                {
                                    Console.WriteLine(" Thread {0} for user {1} has been cancelled", item.ThreadId, item.UserName);
                                }
                                else
                                {
                                    Console.WriteLine(" Unable to cancel thread {0}", item.ThreadId);
                                }
                            }

                            // Wait so state can be updated
                            Console.WriteLine(" Waiting for 1 second for threads to cancel");
                            System.Threading.Thread.Sleep(1000);

                            // Get the state again to check for more threads to cancel
                            items = GetState(hTop, hTopPool, hConnection);
                            toCancel = GetThreadsToCancel(items);

                            if (toCancel.Count == 0)
                            {
                                // No more items to cancel so exit
                                break;
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("No threads to cancel");
                    }
                    Console.WriteLine("Cancelling of threads complete.");
                }

                if (bDisconnect)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Disconnecting users...");
                    List<TopItem> toDisconnect = GetUsersToDisconnect(items, sUserName);
                    if(toDisconnect.Count > 0)
                    {
                        foreach (TopItem item in toDisconnect)
                        {
                            bool result = DisconnectUser(hUser, hPool, hServer, item.UserName);
                            if (result)
                            {
                                Console.WriteLine(" User {0} has been disconnected", item.UserName);
                            }
                            else
                            {
                                Console.WriteLine(" Unable to disconnect user {0}", item.UserName);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No users to disconnected");
                    }
                    Console.WriteLine("Disconnection of users complete.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (hPool != TM1V.Zero)
                {
                    try
                    {
                        API.TM1ValPoolDestroy(hPool);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to destroy main pool: " + ex.ToString());
                    }
                }
                if (hUser != TM1V.Zero)
                {
                    try
                    {
                        API.TM1SystemClose(hUser);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to close main system: " + ex.ToString());
                    }
                }
                if (hTopPool != TM1V.Zero)
                {
                    try
                    {
                        API.TM1ValPoolDestroy(hTopPool);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to destroy top pool: " + ex.ToString());
                    }
                }
                if (hTop != TM1V.Zero)
                {
                    try
                    {
                        API.TM1SystemClose(hTop);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to close top system: " + ex.ToString());
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("COMPLETE!");

        }

        private static uint GetThreadID(string possibleThreadID)
        {
            ulong tId = ulong.Parse(possibleThreadID);
            uint finalThreadID = 0;

            if (!String.IsNullOrEmpty(possibleThreadID))
            {
                if (tId > uint.MaxValue)
                {
                    String lastDigits = possibleThreadID.Substring(possibleThreadID.Length - 9, 9);
                    finalThreadID = uint.Parse(lastDigits);
                }
                else
                {
                    finalThreadID = uint.Parse(possibleThreadID);
                }
            }

            return finalThreadID;
        }

        private static List<TopItem> GetState(TM1V hUser, TM1V hPool, TM1V hConnection)
        {

            List<TopItem> result = new List<TopItem>();

            // Get the current state from the TOP API
            TM1V hState = API.TM1TopGetCurrentState(hPool, hConnection);
            if (API.TM1ValType(hUser, hState) == API.TM1ValTypeError())
            {
                Console.WriteLine("Unable to get state from TM1 Top: " + API.TM1ValErrorString(hUser, hState));
                return result;
            }

            // Loop through the state
            for (uint i = 1; i < uint.MaxValue; i++)
            {

                TM1V hLine = API.TM1ValArrayGet(hUser, hState, i);
                if (API.TM1ValType(hUser, hLine) != API.TM1ValTypeString())
                {
                    // No more items so break loop
                    break;
                }
                string line = API.TM1ValStringGet(hUser, hLine);
                if (!string.IsNullOrEmpty(line))
                {
                    line = line.Trim();
                    if (line.Length > 0)
                    {
                        string[] columns = line.Split('\t');
                        TopItem item = new TopItem();
                        item.ThreadId = GetThreadID(columns[0]);
                        item.UserName = columns[1];
                        item.State = columns[2];
                        result.Add(item);
                    }
                }

            }

            return result;
        }

        private static List<TopItem> GetThreadsToCancel(List<TopItem> items)
        {
            List<TopItem> result = new List<TopItem>();
            foreach (TopItem item in items)
            {
                // Cancel all non-system threads that aren't idle
                if (!item.UserName.ToLower().StartsWith("th:") && !item.State.ToLower().StartsWith("idle"))
                {
                    result.Add(item);
                }
               
            }
            return result;
        }

        private static List<TopItem> GetUsersToDisconnect(List<TopItem> items, String sUserName)
        {
            List<TopItem> result = new List<TopItem>();
            foreach (TopItem item in items)
            {
                // Don't disconnect system threads or the current user
                if (!item.UserName.ToLower().StartsWith("th:") && !item.UserName.Equals(sUserName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private static bool CancelThread(TM1V hUser, TM1V hPool, TM1V hConnection, uint threadId)
        {

            try
            {

                TM1V hThreadId = API.TM1ValIndex(hPool, threadId);
                TM1V hResult = API.TM1TopKillTiProcess(hPool, hConnection, hThreadId);

                if (API.TM1ValType(hUser, hResult) == API.TM1ValTypeError())
                {
                    string error = API.TM1ValErrorString(hUser, hResult);
                    Console.WriteLine("Could not kill thread {0}: {1}", threadId, error);
                    return false;
                }
                else if (API.TM1ValType(hUser, hResult) == API.TM1ValTypeBool())
                {
                    bool result = API.TM1ValBoolGet(hUser, hResult);
                    return result;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while cancelling thread: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return false;

        }

        private static bool DisconnectUser(TM1V hUser, TM1V hPool, TM1V hServer, String userName)
        {

            try
            {
                TM1V hResult = API.TM1ClientDisconnect(hPool, hServer, API.TM1ValString(hPool, userName, 0));
                if (API.TM1ValType(hUser, hResult) == API.TM1ValTypeError())
                {
                    string error = API.TM1ValErrorString(hUser, hResult);
                    Console.WriteLine("Could not disconnect user thread {0}: {1}", userName, error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while disconnecting user: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }

        }

    }
}
