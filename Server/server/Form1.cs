using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {

        // Different mutexes are needed since threads.
        // Mutexes are used when threads enter critical regions
        private readonly Mutex m = new Mutex();
        private readonly Mutex m2 = new Mutex();
        private readonly Mutex m3 = new Mutex();
        private readonly Mutex m4 = new Mutex();
        //########################################################


        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //list of clients
        List<Socket> clientSockets = new List<Socket>();
        List<String> onlineClients = new List<String>();
        List<KeyValuePair<String, Socket>> kicklist = new List<KeyValuePair<String, Socket>>();
        //########################################################

        //file database array
        List<FileOfClient> AllFiles = new List<FileOfClient>();
        //########################################################

        //global checkers
        bool terminating = false;
        bool listening = false;
        const int sizeByte = 1024;
        string path = "";
        //########################################################


        class FileOfClient // file class stored in database
        {
            public string uName { get; set; }
            public string fName { get; set; }
            public int count { get; set; }
            public string size { get; set; }
            public string date { get; set; }
            public string isPublic { get; set; }
        };

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button_listen_Click(object sender, EventArgs e) // with listen button, it starts listening
        {

            int serverPort;
            if (path != "")
                if (Int32.TryParse(textBox_port.Text, out serverPort))
                {
                    // part that server starts listening
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(3);
                    //########################################################

                    //GUI changes after start listening
                    listening = true;
                    button_listen.Enabled = false;
                    textBox_message.Enabled = true;
                    button_send.Enabled = true;
                    buttonBrowse.Enabled = false;
                    //########################################################

                    //listens through threads and accepts clients
                    Thread acceptThread = new Thread(Accept);
                    acceptThread.IsBackground = true;
                    acceptThread.Start();
                    logs.AppendText("Started listening on port: " + serverPort + "\n");
                    //########################################################

                    //Database DB.txt, location
                    string dbPath = path + "DB.txt";
                    if (!File.Exists(dbPath))
                        File.Create(dbPath).Close(); //if not exist create DB.txt
                    List<string> filelines = File.ReadAllLines(dbPath).ToList(); // read all database as lines
                                                                                 //########################################################

                    //read database & save it to array for later use
                    foreach (string line in filelines)
                    {
                        string[] parts = line.Split(';');

                        FileOfClient file = new FileOfClient();
                        file.uName = parts[0];
                        file.fName = parts[1];
                        file.size = parts[3];
                        file.date = parts[4];
                        file.count = Convert.ToInt32(parts[2]);
                        file.isPublic = parts[5];

                        AllFiles.Add(file);
                    }
                    //########################################################

                }
                else
                {
                    logs.AppendText("Please check port number \n");
                }
            else
                logs.AppendText("Please check location \n");
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {
                    //accept client as connection
                    Socket newClient = serverSocket.Accept();

                    //check if accepted client is already 
                    //used mutex since onlineClients array is critical region
                    m4.WaitOne();
                    try
                    {
                        //receive name of the client (USSR)
                        byte[] buffer = new byte[sizeByte];
                        newClient.Receive(buffer);
                        string USSR = Encoding.UTF8.GetString(buffer);
                        USSR = USSR.Substring(0, USSR.IndexOf("\0"));
                        //########################################################

                        //find if USSR already online
                        string result = onlineClients.Find(item => item == USSR);
                        if (result != null) //if online, kick client
                        {
                            string message = "500 This client is already connected. Kicked out\n";
                            Byte[] buffer2 = Encoding.Default.GetBytes(message);
                            newClient.Send(buffer2);
                            newClient.Close();
                            logs.AppendText("Client " + USSR + " tried to connect again\n");
                        }
                        else //if not online, add the client to arrays to further listen them
                        {//these arrays are used in following functions
                            clientSockets.Add(newClient);
                            onlineClients.Add(USSR);
                            kicklist.Add(new KeyValuePair<String, Socket>(USSR, newClient));
                            checkedListBox1.Items.Add(USSR, false);
                            logs.AppendText("A client is connected.\n");
                            Thread receiveThread = new Thread(() => ReciveFile(newClient, USSR)); // updated
                            receiveThread.IsBackground = true;
                            receiveThread.Start();
                        }
                        //########################################################
                    }
                    finally
                    {
                        m4.ReleaseMutex();
                    }

                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        logs.AppendText("The socket stopped working.\n");
                    }

                }
            }
        }


        /*
        private void Receive(Socket thisClient) // updated
        {
            bool connected = true;

            while (connected && !terminating)
            {
                try
                {
                    Byte[] buffer = new Byte[1024];
                    thisClient.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    logs.AppendText("Client: " + incomingMessage + "\n");
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("A client has disconnected\n");
                    }
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                    connected = false;
                }
            }
        }
        */

        private void ReciveFile(Socket thisClient, string USSR) //receiving file of client
        {
            bool connected = true;
            while (connected && !terminating)//run if client is connected& server is not terminating
            {
                try
                {
                    //Decoding received buffer. data is received in
                    //<username>:<size>:<filename>
                    byte[] b = new byte[sizeByte];
                    int rec = 1;
                    rec = thisClient.Receive(b);

                    int index;
                    for (index = 0; index < b.Length; index++)
                        if (b[index] == 63)
                            break;
                    string incomingPacket = Encoding.UTF8.GetString(b.Take(index).ToArray());
                    string statusCode = incomingPacket.Substring(0, 3);
                    string username = kicklist.FirstOrDefault(kvp => kvp.Value == thisClient).Key;

                    string incomingMessage = incomingPacket.Substring(3);

                    if (statusCode == "220") //recieving file
                    {
                        string[] fInfo = incomingMessage.Split(':');
                        //########################################################

                        // Constructing filename: username+filename 
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string filefull = path + fInfo[0] + fInfo[2];
                        string filefull2 = path + fInfo[0] + fInfo[2];
                        //########################################################

                        int count = 0;
                        m.WaitOne(); // mutex is used because database updated here.
                        try
                        {   // construction filename contd. file numbers added for multiple submission
                            // username+filename+count+.txt
                            while (File.Exists(filefull2))
                            {
                                if (count <= 9)
                                    filefull2 = filefull.Substring(0, filefull.Length - 4) + "-0" + (count + 1).ToString() + ".txt";
                                else
                                    filefull2 = filefull.Substring(0, filefull.Length - 4) + "-" + (count + 1).ToString() + ".txt";
                                count++;
                            }
                            //########################################################

                            //adding file to database array and database txt.
                            FileOfClient newFile = new FileOfClient();
                            DateTime now = DateTime.UtcNow;
                            newFile.fName = fInfo[2];
                            newFile.uName = fInfo[0];
                            newFile.size = fInfo[1];
                            newFile.date = now.ToString();
                            newFile.count = count;
                            newFile.isPublic = "Private";
                            AllFiles.Add(newFile);
                            string willAdded = newFile.uName + ";" + newFile.fName + ";" + newFile.count + ";" + newFile.size + ";" + newFile.date + ";" + newFile.isPublic + ";\n";
                            File.AppendAllText(path + "DB.txt", willAdded);
                            //
                        }
                        finally
                        {
                            m.ReleaseMutex();
                        }


                        //filefull2 constructed filename of the file for the server
                        //using filestream in order to write file to server's folder.
                        FileStream fs = new FileStream(filefull2, FileMode.Append, FileAccess.Write);
                        string strEnd;
                        while (true)
                        {

                            // buffer reached to end of the file, empty filestream and close filestream.
                            rec = thisClient.Receive(b);
                            strEnd = ((char)b[0]).ToString() + ((char)b[1]).ToString() + ((char)b[2]).ToString() + ((char)b[3]).ToString() + ((char)b[4]).ToString() + ((char)b[5]).ToString();
                            if (strEnd == "!endf!")
                            {
                                m2.WaitOne();
                                try
                                {
                                    fs.Flush();
                                    fs.Close();
                                }
                                finally
                                {
                                    m2.ReleaseMutex();
                                }

                                logs.AppendText("Received File From: " + fInfo[0] + " \t" + ((float)(float.Parse(fInfo[1]) / 1024)).ToString() + "  KB\n");
                                break;
                            }
                            //########################################################

                            //if it is not end of the file, write to the file.
                            m2.WaitOne();
                            try
                            {
                                fs.Write(b, 0, rec);
                            }
                            finally
                            {
                                m2.ReleaseMutex();

                            }
                            //########################################################
                        }
                    }
                    else if (statusCode == "240")//File request
                    {
                        logs.AppendText("\n<" + statusCode + ">\n" + incomingMessage.Substring(0, incomingMessage.IndexOf('\0')) + "\n");//LOOOOOKKK ATTTT

                        string sendLine = "240";
                        foreach (FileOfClient line in AllFiles)
                        {
                            if (line.uName == username)
                                sendLine += line.uName + ";" + line.fName + ";" + line.count.ToString() + ";" + line.size + ";" + line.date + ";" + line.isPublic + "\n";
                        }
                        Byte[] buffer2 = Encoding.Default.GetBytes(sendLine);
                        thisClient.Send(buffer2);

                    }
                    else if (statusCode == "241")//Public file request
                    {
                        logs.AppendText("\n<" + statusCode + ">\n" + incomingMessage.Substring(0, incomingMessage.IndexOf('\0')) + "\n");//LOOOOOKKK ATTTT

                        string sendLine = "241";
                        foreach (FileOfClient line in AllFiles)
                        {
                            if (line.isPublic == "Public")
                                sendLine += line.uName + ";" + line.fName + ";" + line.count.ToString() + ";" + line.size + ";" + line.date + ";" + line.isPublic + "\n";
                        }
                        Byte[] buffer2 = Encoding.Default.GetBytes(sendLine);
                        thisClient.Send(buffer2);

                    }
                    else if (statusCode == "260")//Download request/sending requested files to client
                    {
                        logs.AppendText("\n<" + statusCode + ">\n" + incomingMessage + "\n");

                        string[] filelines = incomingMessage.Substring(0, incomingMessage.IndexOf('\0')).Split('\n'); // read all database as lines
                                                                                                                      //########################################################

                        //read database & save it to array for later use
                        List<string> paths = new List<string>();
                        List<string> userNames = new List<string>();
                        foreach (string line in filelines)
                        {
                            if (line != "")
                            {
                                string[] pathWillAdd = line.Split(';');
                                userNames.Add(pathWillAdd[0]);
                                if (pathWillAdd[2] == "0")
                                {
                                    paths.Add(path + pathWillAdd[0] + pathWillAdd[1]);
                                    
                                }
                                else
                                {
                                    string fileName = pathWillAdd[1].Substring(0, pathWillAdd[1].IndexOf('.'));
                                    if (int.Parse(pathWillAdd[2]) <= 9)
                                    {
                                        fileName += "-0" + pathWillAdd[2];
                                    }
                                    else
                                    {
                                        fileName += "-" + pathWillAdd[2];
                                    }
                                    paths.Add(path + pathWillAdd[0] + fileName + ".txt");
                                }

                            }
                        }

                        for (int i= 0; i< paths.Count; i++)

                        {
                            FileSend(paths[i], userNames[i], thisClient);
                        }


                    }
                    else if (statusCode == "280")//Copy File
                    {
                        logs.AppendText("\n<" + statusCode + ">\n" + incomingMessage + "\n");
                        incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf('\0'));

                        string[] filelines = incomingMessage.Split('\n');

                        //read database & save it to array for later use
                        string[] fileEntry;
                        string filename_tocopy_infolder;
                        string filename_copied_infolder;

                        foreach (string line in filelines)
                        {
                            FileOfClient file = new FileOfClient();
                            if (line != "")
                            {

                                fileEntry = line.Split(';');

                                file.uName = fileEntry[0];
                                if (file.uName == username)
                                {
                                    file.fName = fileEntry[1];
                                    file.count = Int32.Parse(fileEntry[2]);
                                    file.date = fileEntry[4];
                                    file.size = fileEntry[3];
                                    file.isPublic = fileEntry[5]; //buradan emin değiliz
                                    if (file.count != 0)
                                    {
                                        if (file.count <= 9)
                                        {
                                            filename_tocopy_infolder = file.uName +
                                                file.fName.Substring(0, file.fName.Length - 4) +
                                                "-0" +
                                                (file.count).ToString() +
                                                ".txt";
                                        }
                                        else
                                        {
                                            filename_tocopy_infolder = file.uName +
                                                file.fName.Substring(0, file.fName.Length - 4) +
                                                "-" +
                                                (file.count).ToString() +
                                                ".txt";
                                        }
                                    }
                                    else
                                        filename_tocopy_infolder = username + fileEntry[1];



                                    int max = 0;
                                    foreach (FileOfClient entry in AllFiles)
                                    {
                                        if (entry.uName == username && entry.fName == file.fName)
                                        {
                                            if (max < entry.count)
                                            {
                                                max = entry.count;
                                            }

                                        }
                                    }
                                    /*
                                    bool[] arr = new bool[50];
                                    for(int k=0;k< arr.Length; k++) { arr[k] = false; }
                                    int max = 0;
                                    bool newCount = false;
                                    int i = 1;
                                    foreach (FileOfClient entry in AllFiles)
                                    {
                                        if (entry.uName == username && entry.fName == file.fName )
                                        {
                                            arr[i] = true;
                                        }
                                    }
                                    for (int t=1;t<arr.Length;t++)
                                    {
                                        if (arr[t] == false)
                                        {
                                            max = t;
                                            break;
                                        }
                                    }
                                    */
                                    max = max + 1;
                                    if (file.count <= 9)
                                    {
                                        filename_copied_infolder = file.uName +
                                            file.fName.Substring(0, file.fName.Length - 4) +
                                            "-0" +
                                            (max).ToString() +
                                            ".txt";
                                    }
                                    else
                                    {
                                        filename_copied_infolder = file.uName +
                                            file.fName.Substring(0, file.fName.Length - 4) +
                                            "-" +
                                            (max).ToString() +
                                            ".txt";
                                    }


                                    // username is username of this client. 
                                    // find out file count of current file.
                                    // add 1 to max file count and create name.
                                    // copy file.
                                    file.count = max;
                                    AllFiles.Add(file);
                                    string willAdded = file.uName + ";" + file.fName + ";" + file.count.ToString() + ";" + file.size + ";" + file.date + ";" + file.isPublic + ";\n";
                                    File.AppendAllText(path + "DB.txt", willAdded);

                                    logs.AppendText(filename_tocopy_infolder + "\n");
                                    logs.AppendText(filename_copied_infolder + "\n");

                                    try
                                    {
                                        File.Copy(path + filename_tocopy_infolder, path + filename_copied_infolder, true);
                                    }
                                    catch (IOException iox)
                                    {
                                        Console.WriteLine(iox.Message);
                                    }
                                }
                                else
                                {
                                    string message = "400You don't have permission to copy file: " + fileEntry[1] + "\n";
                                    Byte[] buffer2 = Encoding.Default.GetBytes(message);
                                    thisClient.Send(buffer2);
                                }


                            }
                        }
                    }
                    else if (statusCode == "230") //publish file request
                    {
                        logs.AppendText("\n<" + statusCode + ">\n" + incomingMessage + "\n");
                        incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf('\0'));

                        string[] filelines = incomingMessage.Split('\n');
                        foreach (string line in filelines)
                        {
                            if (line != "")
                            {
                                string[] file = line.Split(';');
                                string willPublished = file[0] + ";" + file[1] + ";" + file[2] + ";" + file[3] + ";" + file[4] + ";" + file[5] + ";";

                                string tempFile = Path.GetTempFileName();

                                using (var sr = new StreamReader(path + "DB.txt"))
                                using (var sw = new StreamWriter(tempFile))
                                {
                                    string dbLine;

                                    while ((dbLine = sr.ReadLine()) != null)
                                    {
                                        if (dbLine == willPublished)
                                        {
                                            dbLine = file[0] + ";" + file[1] + ";" + file[2] + ";" + file[3] + ";" + file[4] + ";" + "Public" + ";";
                                        }
                                        sw.WriteLine(dbLine);

                                    }
                                }

                                File.Delete(path + "DB.txt");
                                File.Move(tempFile, path + "DB.txt");
                                AllFiles.Clear();

                                List<string> fileliness = File.ReadAllLines(path + "DB.txt").ToList(); // read all database as lines
                                                                                                       //########################################################

                                //read database & save it to array for later use
                                foreach (string linee in fileliness)
                                {
                                    string[] parts = linee.Split(';');

                                    FileOfClient filee = new FileOfClient();
                                    filee.uName = parts[0];
                                    filee.fName = parts[1];
                                    filee.size = parts[3];
                                    filee.date = parts[4];
                                    filee.count = Convert.ToInt32(parts[2]);
                                    filee.isPublic = parts[5];

                                    AllFiles.Add(filee);
                                }
                            }
                        }


                    }
                    else if (statusCode == "300") //delete file request
                    {
                        logs.AppendText("\n<" + statusCode + ">\n" + incomingMessage + "\n");
                        incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf('\0'));

                        string[] filelines = incomingMessage.Split('\n');

                        //read database & save it to array for later use
                        string[] fileEntry;
                        string filename_todelete_infolder;

                        foreach (string line in filelines)
                        {
                            FileOfClient file = new FileOfClient();
                            if (line != "")
                            {

                                fileEntry = line.Split(';');

                                file.uName = fileEntry[0];
                                if (file.uName == username)
                                {
                                    file.fName = fileEntry[1];
                                    file.count = Int32.Parse(fileEntry[2]);
                                    file.date = fileEntry[4];
                                    file.size = fileEntry[3];
                                    file.isPublic = fileEntry[5];

                                    if (file.count != 0)
                                    {
                                        if (file.count <= 9)
                                        {
                                            filename_todelete_infolder = file.uName +
                                                file.fName.Substring(0, file.fName.Length - 4) +
                                                "-0" +
                                                (file.count).ToString() +
                                                ".txt";
                                        }
                                        else
                                        {
                                            filename_todelete_infolder = file.uName +
                                                file.fName.Substring(0, file.fName.Length - 4) +
                                                "-" +
                                                (file.count).ToString() +
                                                ".txt";
                                        }
                                    }
                                    else
                                        filename_todelete_infolder = username + fileEntry[1];




                                    string willRemoved = file.uName + ";" + file.fName + ";" + file.count.ToString() + ";" + file.size + ";" + file.date + ";" + file.isPublic + ";";

                                    string tempFile = Path.GetTempFileName();

                                    using (var sr = new StreamReader(path + "DB.txt"))
                                    using (var sw = new StreamWriter(tempFile))
                                    {
                                        string dbLine;

                                        while ((dbLine = sr.ReadLine()) != null)
                                        {
                                            if (dbLine != willRemoved)
                                                sw.WriteLine(dbLine);
                                        }
                                    }

                                    File.Delete(path + "DB.txt");
                                    File.Move(tempFile, path + "DB.txt");

                                    AllFiles.Clear();

                                    List<string> fileliness = File.ReadAllLines(path + "DB.txt").ToList(); // read all database as lines
                                                                                                           //########################################################

                                    //read database & save it to array for later use
                                    foreach (string linee in fileliness)
                                    {
                                        string[] parts = linee.Split(';');

                                        FileOfClient filee = new FileOfClient();
                                        filee.uName = parts[0];
                                        filee.fName = parts[1];
                                        filee.size = parts[3];
                                        filee.date = parts[4];
                                        filee.count = Convert.ToInt32(parts[2]);
                                        filee.isPublic = parts[5];

                                        AllFiles.Add(filee);
                                    }

                                    try
                                    {
                                        File.Delete(path + filename_todelete_infolder);
                                    }
                                    catch (IOException iox)
                                    {
                                        Console.WriteLine(iox.Message);
                                    }
                                }
                                else
                                {
                                    string message = "400You don't have permission to delete file: " + fileEntry[1] + "\n";
                                    Byte[] buffer2 = Encoding.Default.GetBytes(message);
                                    thisClient.Send(buffer2);
                                }



                            }
                            // username is username of this client.
                            // find file.
                            // in database, delete it
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!terminating)
                    {
                        //if client is lost, remove client form arrays.
                        logs.AppendText("A client has disconnected\n");
                        kicklist.RemoveAll(kvp => kvp.Key == USSR);
                        checkedListBox1.Items.Remove(USSR);
                        onlineClients.Remove(USSR);
                    }
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                    connected = false;

                }
            }
        }
        private void FileSend(object filePathFull, string username, Socket clientSocket)
        {
            try
            {
                FileInfo inf = new FileInfo((string)filePathFull);
                //Make a progress bar to see the progress of sending the file.

                //Files are sent by FileStream class
                FileStream f = new FileStream((string)filePathFull, FileMode.Open);
                //To identify the data from each other put ":" and send username, file length, name
                byte[] statusCode = Encoding.UTF8.GetBytes("220");
                byte[] fusername = Encoding.UTF8.GetBytes(username + ":");
                byte[] fsize = Encoding.UTF8.GetBytes(inf.Length.ToString() + ":");
                byte[] fname = Encoding.UTF8.GetBytes(inf.Name + "?");
                byte[] fInfo = new byte[sizeByte];
                statusCode.CopyTo(fInfo, 0);
                fusername.CopyTo(fInfo, statusCode.Length);
                fsize.CopyTo(fInfo, statusCode.Length + fusername.Length);
                fname.CopyTo(fInfo, statusCode.Length + fsize.Length + fusername.Length);
                clientSocket.Send(fInfo);
                if (sizeByte > f.Length)
                {
                    byte[] b = new byte[f.Length];
                    f.Seek(0, SeekOrigin.Begin);
                    f.Read(b, 0, (int)f.Length);
                    clientSocket.Send(b);
                }
                else
                {
                    for (int i = 0; i < (f.Length - sizeByte); i = i + sizeByte)
                    {
                        byte[] b = new byte[sizeByte];
                        f.Seek(i, SeekOrigin.Begin);
                        f.Read(b, 0, b.Length);
                        clientSocket.Send(b);

                        if (i + sizeByte >= f.Length - sizeByte)
                        {
                            int ind = (int)f.Length - (i + sizeByte);
                            byte[] ed = new byte[ind];
                            f.Seek(i + sizeByte, SeekOrigin.Begin);
                            f.Read(ed, 0, ed.Length);
                            clientSocket.Send(ed);
                        }
                    }

                }
                //Close the filestream and finish the operation.
                f.Close();
                Thread.Sleep(1000);
                clientSocket.Send(Encoding.UTF8.GetBytes("!endf!"));
                Thread.Sleep(1000);

                //Inform user that the file is sent and give the size of the file.
                logs.AppendText("Send File " + ((float)inf.Length / 1024).ToString() + "  KB\n");
                //MessageBox.Show("Send File " + ((float)inf.Length / 1024).ToString() + "  KB");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void button_send_Click(object sender, EventArgs e) // just to send some messages manually.
        {
            string message = textBox_message.Text;
            if (message != "" && message.Length <= 1024)
            {
                Byte[] buffer = Encoding.Default.GetBytes(message);
                foreach (Socket client in clientSockets)
                {
                    try
                    {
                        client.Send(buffer);
                    }
                    catch
                    {
                        logs.AppendText("There is a problem! Check the connection...\n");
                        terminating = true;
                        textBox_message.Enabled = false;
                        button_send.Enabled = false;
                        textBox_port.Enabled = true;
                        button_listen.Enabled = true;
                        serverSocket.Close();
                    }

                }
            }
        }



        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        /*
         
        
                 ______ _    _ _   _     _____        _____ _______ 
                |  ____| |  | | \ | |   |  __ \ /\   |  __ \__   __|
                | |__  | |  | |  \| |   | |__) /  \  | |__) | | |   
                |  __| | |  | | . ` |   |  ___/ /\ \ |  _  /  | |   
                | |    | |__| | |\  |   | |  / ____ \| | \ \  | |   
                |_|     \____/|_| \_|   |_| /_/    \_\_|  \_\ |_|   
                                                  

         */
        private void buttonKick_Click(object sender, EventArgs e) // we added a checkboxlist to be able to kick some users, we kicked nureddin first.
        {
            List<string> x2 = checkedListBox1.CheckedItems.OfType<string>().ToList();
            foreach (string name in x2) // kick each checked user from the server hehe
            {
                checkedListBox1.Items.Remove(name);
                string message = "500 Kicked out\n";
                Byte[] buffer2 = Encoding.Default.GetBytes(message);
                kicklist.FirstOrDefault(kvp => kvp.Key == name).Value.Send(buffer2);
                kicklist.RemoveAll(kvp => kvp.Key == name);
                onlineClients.RemoveAll(kvp => kvp == name);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e) // this is just opens a file dialog and selects a path.
        {
            int size = -1;
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string filePathFull = folderBrowserDialog1.SelectedPath; // filePathFull = filePath + fileName 
                textBoxLocation.Text = filePathFull;
                path = filePathFull + "\\";

            }
        }
    }
}
