using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Lanceur
{
    class Program
    {
        public static string source = ConfigurationSettings.AppSettings["Source"];
        public static string result = ConfigurationSettings.AppSettings["Result"];
        public static string serveurDest = ConfigurationSettings.AppSettings["ServeurDest"];
        public static string pathDestSource = ConfigurationSettings.AppSettings["PathDestSource"];
        public static string pathDestResult = ConfigurationSettings.AppSettings["PathDestResult"];
        public static string programExec = ConfigurationSettings.AppSettings["ProgramExec"];
        public static string login = ConfigurationSettings.AppSettings["Login"];
        public static string password = ConfigurationSettings.AppSettings["Password"];
        public static string psexec = ConfigurationSettings.AppSettings["PsExec"];
        public static string codehex = Dns.GetHostName().Substring(2, 4);

        static void Main(string[] args)
        {
            string linkDestSource = @"\\" + serveurDest.Replace("%codehex%", codehex)+@"\"+pathDestSource;
            string linkDestResult = @"\\" + serveurDest.Replace("%codehex%", codehex) + @"\" + pathDestResult;

            #region purge des répertoires distant
            try
            {
                if (Directory.Exists(linkDestSource))
                {
                    string[] filenames = Directory.GetFiles(linkDestSource, "*", SearchOption.TopDirectoryOnly);
                    foreach (string fName in filenames)
                    {
                        File.Delete(fName);
                    }
                    Directory.Delete(linkDestSource);
                }
                if (Directory.Exists(linkDestResult))
                {
                    string[] filenames = Directory.GetFiles(linkDestResult, "*", SearchOption.TopDirectoryOnly);
                    foreach (string fName in filenames)
                    {
                        File.Delete(fName);
                    }
                    Directory.Delete(linkDestResult);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            #endregion

            #region création répertoire
            Console.WriteLine("Création des répertoire distant");
            try
            {
                if (!Directory.Exists(linkDestSource))
                {
                    Directory.CreateDirectory(linkDestSource);
                }
                if (!Directory.Exists(linkDestResult))
                {
                    Directory.CreateDirectory(linkDestResult);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            #endregion

            #region copie des fichiers sur serveur distant
            try
            {
                Console.WriteLine("Copie des fichiers");
                string[] filenames = Directory.GetFiles(source, "*", SearchOption.TopDirectoryOnly);
                if (filenames.Length>0)
                {
                    foreach (string fName in filenames)
                    {
                        Console.WriteLine(fName);
                        Console.WriteLine(fName.Substring(fName.LastIndexOf(("\\")) + 1));
                        Console.WriteLine("vers : "+linkDestSource + fName.Substring(fName.LastIndexOf((@"\")) + 1)) ;
                        File.Copy(fName, linkDestSource + fName.Substring(fName.LastIndexOf((@"\")) + 1));
                    }
                }
                else
                {
                    Console.WriteLine("Pas de fichier à copier");
                }
                
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            #endregion

            #region kill psexec
            Console.WriteLine("Kill psexec");
            try
            {

                File.WriteAllText(source+"cmdKillProcess.cmd",@"TASKKILL /S \\" + serveurDest.Replace("%codehex%", codehex) + " /FI \" IMAGENAME EQ psexesvc.exe\"");
                Process p = new Process();
                p.StartInfo.Arguments = @"TASKKILL /S \\" + serveurDest.Replace("%codehex%", codehex) + " /FI \" IMAGENAME EQ psexesvc.exe\"";
                if (File.Exists(source + "cmdKillProcess.cmd"))
                {
                    p.StartInfo.FileName = source + "cmdKillProcess.cmd";
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.Start();
                    p.WaitForExit(10000);
                }
                else
                {
                    Console.WriteLine("cmd pour kill introuvable : " + source + "cmdKillProcess.cmd");
                }
                
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            
            #endregion

            #region exec psexec
            Console.WriteLine("Lancement du psexec");
            try
            {
                if (File.Exists(linkDestSource+programExec))
                {
                    if (File.Exists(psexec))
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = psexec;
                        p.StartInfo.Arguments = @"\\"+serveurDest.Replace("%codehex%", codehex) + " -S -ACCEPTEULA " + linkDestSource + programExec;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.Start();
                        p.WaitForExit(240000);
                    }
                    else
                    {
                        Console.WriteLine("Psexec non présent");
                    }
                    
                }
                else
                {
                    Console.WriteLine("Le fichier à exécuter est introuvable : " + linkDestSource + programExec);
                }
                
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            #endregion

            #region récupération du résultat
            if (args.Length > 0)
            {
                if (args[0] == "1") //récupération à faire
                {
                    Console.WriteLine("Récupération des fichiers résultat");
                    try
                    {
                        string[] filenames = Directory.GetFiles(linkDestResult, "*", SearchOption.TopDirectoryOnly);
                        foreach (string fName in filenames)
                        {
                            File.Copy(fName, result + fName.Substring(fName.LastIndexOf(("\\")) + 1));
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                }
            }
            #endregion

            #region suppression des rpertoire distant
            try
            {
                if (Directory.Exists(linkDestSource))
                {
                    string[] filenames = Directory.GetFiles(linkDestSource, "*", SearchOption.TopDirectoryOnly);
                    foreach (string fName in filenames)
                    {
                        File.Delete(fName);
                    }
                    Directory.Delete(linkDestSource);
                }
                if (Directory.Exists(linkDestResult))
                {
                    string[] filenames = Directory.GetFiles(linkDestResult, "*", SearchOption.TopDirectoryOnly);
                    foreach (string fName in filenames)
                    {
                        File.Delete(fName);
                    }
                    Directory.Delete(linkDestResult);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            #endregion
        }
    }
}
