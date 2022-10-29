using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageClient {
    internal class Connection {
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
            _client.serverRSA = new Crypto(buffer);
            _client.WriteMessage(Encoding.UTF8.GetString(buffer));
            SendMessage("Hello, World!");
            Task.Run(RecieveMessage);
        }

        private void RecieveMessage() { 
            
        }

        private void SendMessage(string message) {
            byte[] buffer = _client.serverRSA.EncryptString(message);
            byte[] send = new byte[buffer.Length + 5];
            for (int i = 0; i < buffer.Length; i++) {
                send[i+1] = buffer[i];
            }
            stream.Write(send, 0, send.Length);
        }
    }
}
