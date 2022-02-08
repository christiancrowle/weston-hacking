using System.IO;
using System.Reflection;

namespace WestonClient {
    public static class Helpers {
        private static string exeDir() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string Res(string path) {
            string realPath = exeDir() + "/res/" + path;
            Console.WriteLine("getting resource @ " + path + " => " + realPath);
            return realPath;
        }

        public static string Root(string path) {
            string realPath = exeDir() + "/" + path;
            Console.WriteLine("getting root file @ " + path + " => " + realPath);
            return realPath;
        }
    }
}