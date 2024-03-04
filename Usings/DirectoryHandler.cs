using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Usings
{
    public class DirectoryHandler
    {
        private DirectoryInfo path;

        public DirectoryInfo Path { 
            get { 
                return path;
            } 
            set {
                if (Directory.Exists(value.FullName))
                {
                    path = value;
                }
            }
        }

        public DirectoryHandler(string path)
        {
            if (!Directory.Exists(path)) throw new ArgumentException();
            this.path = new DirectoryInfo(path);
        }

        public void MoveBack()
        {
            try
            {
                if(path.FullName == path.Root.FullName) throw new ArgumentException();

                path = path.Parent!;
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void MoveIn(string directoryName)
        {
            try
            {
                //remove '.' until none left
                while (directoryName.Contains('.'))
                {
                    MoveBack();
                    directoryName.Remove(0, 1);
                }
                if (directoryName.Contains(':'))
                {
                    string toSetPath = directoryName;
                    if (Directory.Exists(toSetPath)) {
                        path = new DirectoryInfo(toSetPath);
                    }
                }
                else
                {
                    string toSetPath = Path + $"/{directoryName}";

                }
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

    }
}
