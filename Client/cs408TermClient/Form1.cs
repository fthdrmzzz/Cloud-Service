using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cs408TermClient
{

    public partial class Form1 : Form

    {
        class FileOfClient // file class stored in database
        {
            public string uName { get; set; }
            public string fName { get; set; }
            public int count { get; set; }
            public string size { get; set; }
            public string date { get; set; }
        };
        bool terminating = false;
        bool connected = false;
        const int sizeByte = 1024; // we are sending txt file 1 kb at a time 

        Socket clientSocket;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            //Form closing feature. Default code.
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing); //handling closing window case
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)//handling closing window case
        {
            //When we close the form.We disconnect and terminate.
            connected = false;
            terminating = true;

            Environment.Exit(0);
        }
        //Connect button
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //Create a new client socket.
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Get the IP input from the form.
            string IP = textBoxIP.Text;

            int portNum;
            if (textBoxName.Text != "")
            {
                if (Int32.TryParse(textBoxPort.Text, out portNum))
                {
                    try
                    {
                        //Connect to the input IP and portNum
                        clientSocket.Connect(IP, portNum);
                        // GUI adjusted
                        buttonConnect.Enabled = false;
                        textBoxIP.Enabled = false;
                        textBoxName.Enabled = false;
                        textBoxPort.Enabled = false;
                        log.Enabled = true;
                        buttonDisconnect.Enabled = true;
                        buttonBrowse.Enabled = true;
                        buttonSend.Enabled = true;
                        button_file_req.Enabled = true;
                        textboxPublish.Enabled = true;
                        buttonPublicFileReq.Enabled = true;

                        // GUI adjusted
                        connected = true;

                        //Inform the client that he connected to the server
                        log.AppendText("Connected to the server!\n");
                        //Create a byte buffer and initialize it by the username.
                        byte[] fusername = Encoding.UTF8.GetBytes(textBoxName.Text);//username
                        //Send username to server side.
                        clientSocket.Send(fusername); // sending name to check whether connected before

                        //Call receive thread.
                        Thread receiveThread = new Thread(Receive);
                        receiveThread.IsBackground = true;
                        receiveThread.Start();

                    }
                    catch
                    {
                        log.AppendText("Could not connect to the server!\n");
                    }
                }
                else
                {
                    log.AppendText("Check the port\n");
                }
            }
            else
            {
                log.AppendText("Name cannot be empty\n");
            }
        }

        private void Receive()
        {
            //While connected, keep receiving available data.
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[1024];
                    clientSocket.Receive(buffer);
                    //Get incoming message and transform to string.
                    string incomingPacket = Encoding.Default.GetString(buffer);
                    //There may be empty characters at the end of the string, get rid of them
                    incomingPacket = incomingPacket.Substring(0, incomingPacket.IndexOf("\0"));

                    string statusCode = incomingPacket.Substring(0, 3);
                    string incomingMessage = incomingPacket.Substring(3);
                    if (statusCode == "500") //kicked
                    {
                        log.AppendText("Server: " + incomingPacket + "\n");
                        //If the client is already in the server, dont go on with this client

                        connected = false;
                        clientSocket.Close();

                        buttonConnect.Enabled = true;
                        buttonDisconnect.Enabled = false;
                        buttonBrowse.Enabled = false;
                        buttonSend.Enabled = false;

                        textBoxIP.Enabled = true;
                        textBoxName.Enabled = true;
                        textBoxPort.Enabled = true;
                    }
                    else if (statusCode == "240" || statusCode == "241")//file list request
                    {
                        checkedListBox_rec_files.Items.Clear();

                        string[] filelines = incomingMessage.Split('\n'); // read all database as lines
                                                                          //########################################################

                        //read database & save it to array for later use
                        string toPrint;
                        foreach (string line in filelines)
                        {
                            if (line != "")
                            {

                                toPrint = line.Replace(';', '\t');
                                checkedListBox_rec_files.Items.Add(toPrint);

                            }
                        }
                    }
                    else if(statusCode == "400")
                    {
                        log.AppendText("Server: "+ incomingMessage );
                    }
                    
                    else if (statusCode == "220")
                    {
                        string path = textBox_browse_down.Text;


                        // Rest is taken from server part
                        string[] fInfo = incomingMessage.Split(':');
                        //########################################################

                        // Constructing filename: username+filename 
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string filefull = path +"\\"+  fInfo[2].Substring(fInfo[0].Length, fInfo[2].Length - (fInfo[0].Length) -1 );
                        string filefull2 = path + "\\"+ fInfo[2].Substring(fInfo[0].Length, fInfo[2].Length - (fInfo[0].Length) -1);
                        //########################################################

                        int count = 0;

                        // construction filename contd. file numbers added for multiple submission
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

                        //

                        progressBar1.Invoke((MethodInvoker)delegate
                        {
                            progressBar1.Maximum = int.Parse(fInfo[1]);
                            progressBar1.Value = 0;
                        });

                        //filefull2 constructed filename of the file for the server
                        //using filestream in order to write file to server's folder.
                        FileStream fs = new FileStream(filefull2, FileMode.Append, FileAccess.Write);
                        string strEnd;
                        while (true)
                        {

                            // buffer reached to end of the file, empty filestream and close filestream.
                            int rec = clientSocket.Receive(buffer);
                            strEnd = ((char)buffer[0]).ToString() + ((char)buffer[1]).ToString() + ((char)buffer[2]).ToString() + ((char)buffer[3]).ToString() + ((char)buffer[4]).ToString() + ((char)buffer[5]).ToString();
                            progressBar1.Invoke((MethodInvoker)delegate
                            {
                                //Fill in the progress bar
                                if (progressBar1.Value + 1024 > progressBar1.Maximum)
                                    progressBar1.Value += progressBar1.Maximum - progressBar1.Value;
                                else
                                    progressBar1.Value += 1024;
                              
                            });
                            if (strEnd == "!endf!")
                            {

                                fs.Flush();
                                fs.Close();



                                log.AppendText("Received File From: " + fInfo[0] + " \t" + ((float)(float.Parse(fInfo[1]) / 1024)).ToString() + "  KB\n");
                                break;
                            }
                            //########################################################

                            //if it is not end of the file, write to the file.

                            fs.Write(buffer, 0, rec);

                            //########################################################
                        }
                    }
                }
                //If the server made this client disconnected, close the clientSocket and adjust GUI
                catch
                {
                    if (!terminating)
                    {
                        log.AppendText("The server has disconnected\n");
                        buttonConnect.Enabled = true;
                        buttonDisconnect.Enabled = false;
                        buttonBrowse.Enabled = false;
                        buttonSend.Enabled = false;

                        textBoxIP.Enabled = true;
                        textBoxName.Enabled = true;
                        textBoxPort.Enabled = true;
                    }

                    clientSocket.Close();
                    connected = false;
                }

            }
        }

        //Browse file button
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string filePathFull = openFileDialog1.FileName; // filePathFull = filePath + fileName 
                textBoxLocation.Text = filePathFull;
                try
                {

                    int lenOfFull = filePathFull.Length;
                    int index = filePathFull.LastIndexOf("\\", lenOfFull);
                    string filePath = filePathFull.Substring(0, index);
                    string fileName = filePathFull.Substring(index + 1);

                }
                catch (IOException)
                {
                }

            }
        }

        //When we click send button, this function will be called.
        private void FileSend(object filePathFull, string username)
        {
            try
            {
                FileInfo inf = new FileInfo((string)filePathFull);
                //Make a progress bar to see the progress of sending the file.
                progressBar1.Invoke((MethodInvoker)delegate
                {
                    progressBar1.Maximum = (int)inf.Length;
                    progressBar1.Value = 0;
                });
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
                        progressBar1.Invoke((MethodInvoker)delegate
                        {
                            //Fill in the progress bar
                            progressBar1.Value = i;
                        });
                        if (i + sizeByte >= f.Length - sizeByte)
                        {
                            progressBar1.Invoke((MethodInvoker)delegate
                            {
                                progressBar1.Value = (int)f.Length;
                            });
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
                log.AppendText("Send File " + ((float)inf.Length / 1024).ToString() + "  KB\n");
                //MessageBox.Show("Send File " + ((float)inf.Length / 1024).ToString() + "  KB");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Client clicks disconnect button
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            //Gui adjustment
            connected = false;
            clientSocket.Close();

            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;
            buttonBrowse.Enabled = false;
            buttonSend.Enabled = false;

            textBoxIP.Enabled = true;
            textBoxName.Enabled = true;
            textBoxPort.Enabled = true;
            button_file_req.Enabled = false;
            button_down_file.Enabled = false;
            buttonPublicFileReq.Enabled = false;
            textboxPublish.Enabled = false;
            button_browse_down.Enabled = false;
            textBox_browse_down.Enabled = false;
            checkedListBox_rec_files.Items.Clear();
            checkedListBox_rec_files.Enabled = false;
            buttonCopy.Enabled = false;
            buttonDelete.Enabled = false;
        }


        //Send button is clicked.
        private void buttonSend_Click(object sender, EventArgs e)
        {
            //We are calling the filesend method.
            string name = textBoxName.Text;
            string filePathFull = textBoxLocation.Text;
            FileSend(filePathFull, name);
            //sendMessage(name);
        }

        private void button_file_req_Click(object sender, EventArgs e)
        {
            try
            {
                checkedListBox_rec_files.Enabled = true;
                byte[] FileReq = Encoding.UTF8.GetBytes("240 Send my files");//username
                clientSocket.Send(FileReq);//Send username to server side.
                button_browse_down.Enabled = true;
                button_down_file.Enabled = true;
                buttonPublicFileReq.Enabled = true;
                textboxPublish.Enabled = true;
                buttonCopy.Enabled = true;
                buttonDelete.Enabled = true;
            }
            catch
            {
                log.AppendText("Could not connect to the server!\n");
            }
        }

        private void button_browse_down_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                try
                {

                    string filePathFull = folderBrowserDialog1.SelectedPath; // filePathFull = filePath + fileName 
                    textBox_browse_down.Text = filePathFull;


                }
                catch (IOException)
                {
                }
            }

        }

        private void button_down_file_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> files = checkedListBox_rec_files.CheckedItems.OfType<string>().ToList();
                if (textBox_browse_down.Text != "")
                    if (files.Count == 0)
                    {
                        log.AppendText("Please select at least one file. \n");

                    }
                    else
                    {
                        string message = "260";

                        foreach (string line in files) // kick each checked user from the server hehe
                        {
                            message += line.Replace('\t', ';') + '\n';
                        }
                        Byte[] buffer2 = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer2);
                        for (int i = 0; i < checkedListBox_rec_files.Items.Count; i++)//unselect all
                        {
                            checkedListBox_rec_files.SetItemChecked(i, false);
                        }
                    }
                else
                    log.AppendText("Please select a directory. \n");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> files = checkedListBox_rec_files.CheckedItems.OfType<string>().ToList();
                if (files.Count == 0)
                {
                    log.AppendText("Please select at least one file. \n");

                }
                else
                {
                    string message = "280";

                    foreach (string line in files) // kick each checked user from the server hehe
                    {
                        message += line.Replace('\t', ';') + '\n';
                    }
                    Byte[] buffer2 = Encoding.Default.GetBytes(message);
                    clientSocket.Send(buffer2);
                    for (int i = 0; i < checkedListBox_rec_files.Items.Count; i++)//unselect all
                    {
                        checkedListBox_rec_files.SetItemChecked(i, false);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> files = checkedListBox_rec_files.CheckedItems.OfType<string>().ToList();
                if (files.Count == 0)
                {
                    log.AppendText("Please select at least one file. \n");

                }
                else
                {
                    string message = "300";

                    foreach (string line in files) // kick each checked user from the server hehe
                    {
                        message += line.Replace('\t', ';') + '\n';
                    }
                    Byte[] buffer2 = Encoding.Default.GetBytes(message);
                    clientSocket.Send(buffer2);
                    for (int i = 0; i < checkedListBox_rec_files.Items.Count; i++)//unselect all
                    {
                        checkedListBox_rec_files.SetItemChecked(i, false);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkedListBox_rec_files_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textboxPublish_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> files = checkedListBox_rec_files.CheckedItems.OfType<string>().ToList();
                if (files.Count == 0)
                {
                    log.AppendText("Please select at least one file. \n");

                }
                else
                {
                    string message = "230";

                    foreach (string line in files) // kick each checked user from the server hehe
                    {
                        string[] file = line.Split('\t');
                        if(file[0] == textBoxName.Text)
                            message += line.Replace('\t', ';') + '\n';
                        else
                        {
                            log.AppendText("You do not have permission to publish file: " + file[1] + "\n");
                        }
                    }
                    if(message.Length != 3)
                    {
                        Byte[] buffer2 = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer2);
                    }
                    
                    for (int i = 0; i < checkedListBox_rec_files.Items.Count; i++)//unselect all
                    {
                        checkedListBox_rec_files.SetItemChecked(i, false);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonPublicFileReq_Click(object sender, EventArgs e)
        {
            try
            {
                checkedListBox_rec_files.Enabled = true;
                byte[] FileReq = Encoding.UTF8.GetBytes("241 Send public files");//username
                clientSocket.Send(FileReq);//Send username to server side.
                button_browse_down.Enabled = true;
                button_down_file.Enabled = true;
                buttonPublicFileReq.Enabled = true;
                textboxPublish.Enabled = true;
                buttonCopy.Enabled = true;
                buttonDelete.Enabled = true;
            }
            catch
            {
                log.AppendText("Could not connect to the server!\n");
            }
        }
    }
}
