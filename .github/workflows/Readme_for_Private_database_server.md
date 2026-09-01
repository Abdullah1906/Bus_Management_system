স্টেপ ১: Local Computer থেকে Bastion Host দিয়ে Private EC2-তে লগইন
আপনার local terminal/cmd খুলুন এবং SSH Agent Forwarding অথবা scp/ProxyCommand ব্যবহার করে Private EC2-তে ঢুকুন:
Bash
ssh -A -i /path/to/key.pem ubuntu@<BASTION_PUBLIC_IP>
Bastion-এ ঢোকার পর সেখান থেকে Private EC2-তে ঢুকুন:
Bash
ssh ubuntu@<PRIVATE_EC2_IP>



git repo theke .bak file clone kore nite hobe 
sudo apt update && sudo apt upgrade -y

curl https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor -o /etc/apt/trusted.gpg.d/microsoft.gpg

curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list | sudo tee /etc/apt/sources.list.d/mssql-server-2022.list 
# ২. SQL Server ইনস্টল করুন
 sudo apt update 
sudo apt install -y mssql-server 
# ৩. SQL Server কনফিগার করুন (Developer edition সিলেক্ট করুন এবং SA password সেট করুন) 
sudo /opt/mssql/bin/mssql-conf setup

3
yes
1
Password:Admin1234@
Again password: Admin123@
curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list 
sudo apt update 
sudo ACCEPT_EULA=Y apt install -y mssql-tools18 unixodbc-dev 
echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc 
source ~/.bashrc

cd /tmp
 wget http://archive.ubuntu.com/ubuntu/pool/main/o/openldap/libldap-2.5-0_2.5.20+dfsg-0ubuntu0.22.04.1_amd64.deb 
sudo apt-get install ./libldap-2.5-0_2.5.20+dfsg-0ubuntu0.22.04.1_amd64.deb
ldd /opt/mssql/bin/sqlservr | grep "not found"
sudo systemctl reset-failed mssql-server.service
sudo systemctl start mssql-server 
sudo systemctl status mssql-server
sudo cp /home/ubuntu/BPS.bak /var/opt/mssql/data/
sudo chown mssql:mssql /var/opt/mssql/data/BPS.bak

sqlcmd -S localhost -U sa -P 'Admin1234@' -C -Q "RESTORE DATABASE BPS FROM DISK = '/var/opt/mssql/data/BPS.bak' WITH MOVE 'BPS' TO '/var/opt/mssql/data/BPS.mdf', MOVE 'BPS_log' TO '/var/opt/mssql/data/BPS_log.ldf'"

jodi login failed ase thle 
sudo systemctl stop mssql-server
sudo /opt/mssql/bin/mssql-conf set-sa-password
sudo systemctl start mssql-server
sqlcmd -S localhost -U sa -P 'Admin1234@' -C -Q "RESTORE DATABASE BPS FROM DISK = '/var/opt/mssql/data/BPS.bak' WITH MOVE 'BPS' TO '/var/opt/mssql/data/BPS.mdf', MOVE 'BPS_log' TO '/var/opt/mssql/data/BPS_log.ldf'"
sqlcmd -S localhost -U sa -P 'Admin123#' -C -Q "SELECT name FROM sys.databases;"



###### aita local pc to private ec2 te bastion host diye niye jaowar jonno 
Private ec2 create korar por database ar bak file private ec2 te nite 
scp -i ./abd_bs_ubuntu.pem -o ProxyCommand="ssh -W %h:%p -i ./abd_bs_ubuntu.pem ubuntu@13.212.230.200" ./BPS.bak ubuntu@10.0.141.111:/home/ubuntu/

