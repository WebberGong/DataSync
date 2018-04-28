以管理员身份打开CMD：
cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319

installutil.exe E:\code\开发\同步程序\DataSync\Service\bin\Debug\Service.exe
installutil.exe /u E:\code\开发\同步程序\DataSync\Service\bin\Debug\Service.exe

删除服务：
sc delete DataSyncService