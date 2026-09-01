# Bus_Management_system


# need one public ec2 for bastion host(backend_sg) and one private ec2(database_sg)
# security group
1. bastion_sg
ssh 22 custom 0.0.0.0/0
custom tcp 5000 custom 0.0.0.0/0
http 80 custom 0.0.0.0/0
https 443 custom 0.0.0.0/0
2.backend_sg
ssh 22 custom bastion_sg
mssql 1433 custom bastion_sg private ip

# ci/ cd pipeline 


after ci/cd file run then search
# http://13.212.230.200/swagger
