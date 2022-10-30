using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageClient {
    internal class Connection {
        bool _disposing = false;

        TcpClient client;
        NetworkStream stream;

        Client _client;

        public Connection(Client _client) {
            this._client = _client;
            client = new TcpClient("127.0.0.1", 4434);
            stream = client.GetStream();
        }

        public void Handshake() {
            stream.Write(new byte[] { 10 });
            byte[] buffer = new byte[300];
            int termCount = 0;
            int writtenBytes = 0;
            do {
                buffer[writtenBytes] = (byte)stream.ReadByte();
                if (buffer[writtenBytes] == 0)
                    termCount++;
                else
                    termCount = 0;
                writtenBytes++;
            } while (termCount < 4 && writtenBytes < 300);
            byte[] output = StreamUtils.RemoveTerminator(buffer, writtenBytes);
            _client.serverRSA = new Crypto(output);

            Task.Run(RecieveMessage);

            SendMessage("Test message");
        }

        private void RecieveMessage() {
            while (!_disposing) {
                try {
                    int opcode = stream.ReadByte();
                    if (opcode == 0) {
                        //1024 byte buffer - Arbitrary number, allows for short-mid sized messages to be read
                        byte[] buffer = new byte[1024];
                        //termCount - Terminator count, empty bytes read - 4 in a row to end a message to the server
                        int termCount = 0;
                        int writtenBytes = 0;
                        do {
                            buffer[writtenBytes] = (byte)stream.ReadByte();
                            if (buffer[writtenBytes] == 0)
                                termCount++;
                            else
                                termCount = 0;
                            writtenBytes++;
                        } while (termCount < 4 && writtenBytes < 1024);
                        //stores the message with terminators (trailing empty bytes) stripped
                        byte[] output = StreamUtils.RemoveTerminator(buffer, writtenBytes);

                        _client.WriteMessage("Server: " + Encoding.UTF8.GetString(output));
                    }
                }catch(Exception e) {
                    Disconnect();
                }
            }
        }

        public void SendMessage(string message) {
            byte[] buffer = _client.serverRSA.EncryptString(message);

            byte[] output = StreamUtils.AddOpcode(buffer, 0);
            byte[] send = StreamUtils.AddTerminator(output);

            stream.Write(send, 0, send.Length);
        }

        public void Disconnect() {
            if (client.Connected) {
                byte[] buffer = new byte[0];
                byte[] send = StreamUtils.AddOpcode(buffer, 100);

                stream.Write(send, 0, send.Length);
            }
            _disposing = true;
        }
    }
}
