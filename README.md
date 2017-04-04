# Lauren_Lanceur

Ce lanceur est exécuté par Admin_Pdv_Lauren.exe et est stocké en amont.

> Fichier de config : 
```
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

```

<u><b>Fonctionnement : </b></u>
1. Purge + Suppression des répertoires distants (`string pathDestSource`,`string pathDestResult`)
2. Création des répertoires distant (`string pathDestSource`,`string pathDestResult`)
3. Copie des fichiers présent dans le répertoire local (`string source`) vers le répertoire distant (`string pathDestSource`)
4. Création et exécution d'un cmd en local pour kill process PsExec : `File.WriteAllText(source+"cmdKillProcess.cmd",@"TASKKILL /S \\" + serveurDest.Replace("%codehex%", codehex) + " /FI \" IMAGENAME EQ psexesvc.exe\"");`
5. Exécution du PsExec.exe : `p.StartInfo.Arguments = @"\\"+serveurDest.Replace("%codehex%", codehex) + " -S -ACCEPTEULA " + linkDestSource + programExec;`
6. Si arg[0] exist et égale 1, on rapatrie les fichiers présents dans le répertoire `string pathDestResult` en local sur `string result`
7. Purge + Suppression des répertoires distants