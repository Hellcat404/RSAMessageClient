using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageClient {
    internal class Client {
        public Crypto serverRSA;
        private Connection conn;

        public Client() {
            conn = new Connection(this);
            Task.Run(conn.Handshake);
        }

        public void WriteMessage(string message) { 
            Console.WriteLine(message);
        }

        public void Disconnect() {
            conn.Disconnect();
        }
    }
}
