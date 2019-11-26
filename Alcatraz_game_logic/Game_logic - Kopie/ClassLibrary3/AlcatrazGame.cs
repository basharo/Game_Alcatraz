using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Alcatraz;

namespace AlcatrazGame
{

    public class Test
    {

        public Test()
        {

        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int players = 2;

            ClientClass clientClass = new ClientClass(players);
            List<Client> clientList = new List<Client>();

            Client clientItem = clientClass.initializeClient(1, players);


            List<ClientClass> clientClassList = new List<ClientClass>();
            List<Alcatraz> alcatrazList = new List<Alcatraz>();


            // foreach (Client clientItem in clientList)
            //{
            clientItem.getClientClass();
            clientItem.getAlcatraz();

            //}

            /*    int counterForEachTest = 0;
                foreach (ClientClass clientClassItem in clientClassList)
                {
                    int counterForEachAlcatraz = 0;
                    int iterator = 0;
                    foreach (Alcatraz alcatrazItem in alcatrazList)
                    {
                        if (counterForEachTest != counterForEachAlcatraz)
                        {
                            clientClassItem.setOther(iterator, alcatrazItem);
                            Console.WriteLine("t" + counterForEachTest + ".setOther( " + iterator + "," + counterForEachAlcatraz);
                            iterator++;
                        }
                        counterForEachAlcatraz++;
                    }
                    counterForEachTest++;
                }*/


            clientItem.getAlcatraz().showWindow();
            clientItem.getAlcatraz().addMoveListener(clientItem.getClientClass());
            clientItem.getAlcatraz().getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
            clientItem.getAlcatraz().start();


            Application.Run();
        }

        public static void Test_FormClosed(object sender, FormClosedEventArgs args)
        {
            Environment.Exit(0);
        }

    }
}
