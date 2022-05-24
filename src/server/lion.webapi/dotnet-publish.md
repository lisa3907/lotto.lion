## dotnet publish

$ sudo cp ~/odinsoft-git/solution-team/lottolion/shells/build.lion.webapi.sh /usr/local/bin/.
$ sudo chmod +x /usr/local/bin/build.lion.webapi.sh

## Let’s Encrypt 

$ sudp apt-get -y install cerbot
$ sudo mkdir /var/www/letsencrypt && sudo chgrp www-data letsencrypt
$ sudo mkdir /etc/letsencrypt/configs

$ sudo nano /etc/letsencrypt/configs/lottoapi.odinsoftware.co.kr.conf

```
domains = lottoapi.odinsoftware.co.kr
rsa-key-size = 4096
server = https://acme-v02.api.letsencrypt.org/directory
email = help@odinsoft.co.kr
text = True
authenticator = webroot
webroot-path = /var/www/letsencrypt/
```

$ sudo nano /etc/nginx/sites-available/lottoapi.odinsoftware.co.kr

```
 	server 
	{
		listen *:80;
		server_name lottoapi.odinsoftware.co.kr;

		location /.well-known/acme-challenge 
		{
			root /var/www/letsencrypt;
		}
    }
 
```

$ sudo ln -s /etc/nginx/sites-available/rms.hironic.com /etc/nginx/sites-enabled/rms.hironic.com
$ sudo nginx -t && sudo nginx -s reload

$ sudo certbot --config /etc/letsencrypt/configs/lottoapi.odinsoftware.co.kr.conf certonly

$ sudo crontab -e

```
@weekly certbot renew --quiet --post-hook "systemctl reload nginx"
```

## Nginx for HTTPS

$ sudo nano /etc/nginx/sites-available/lottoapi.odinsoftware.co.kr

```
#
# HTTPS
#
server
{
        listen 443 ssl;
        server_name lottoapi.odinsoftware.co.kr;

        ssl on;
        ssl_certificate /etc/letsencrypt/live/lottoapi.odinsoftware.co.kr/fullchain.pem;
        ssl_certificate_key /etc/letsencrypt/live/lottoapi.odinsoftware.co.kr/privkey.pem;

        location /
        {
            proxy_pass http://localhost:5127;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection keep-alive;
            proxy_set_header Host $host;
            proxy_cache_bypass $http_upgrade;
        }
}

#
# HTTP
#
server
{
        listen 80;
        server_name rms.hironic.com;

        # Let's Encrypt
        location ^~ /.well-known/acme-challenge {
            default_type "text/plain";
            root /var/www/letsencrypt;
        }

        location / {
                 return 301 https://$host$request_uri;
        }
        #rewrite ^ https://$server_name$request_uri? permanent;
}
```

$ sudo nginx -t && sudo nginx -s reload

## supervisor

$ sudo nano /etc/supervisor/conf.d/lion.webapi.conf

```
[program:lion.webapi]
command=/usr/bin/dotnet /var/lion.webapi/lion.webapi.dll --server.urls:http://*:5127
directory=/var/lion.webapi/
autostart=true
autorestart=true
stderr_logfile=/var/log/lion.webapi.err.log
stdout_logfile=/var/log/lion.webapi.out.log
environment=ASPNETCORE_ENVIRONMENT=Production
environment=HOME=/home/odinsoft
user=www-data
stopsignal=INT
```

## logrotate

$ sudo nano /etc/logrotate.d/lion.webapi

```
/var/log/lion.webapi.out.log {
        rotate 4
        weekly
        missingok
        create 640 root adm
        notifempty
        compress
        delaycompress
        postrotate
                /usr/sbin/service supervisor reload > /dev/null
        endscript
}

/var/log/lion.webapi.err.log {
        rotate 4
        weekly
        missingok
        create 640 root adm
        notifempty
        compress
        delaycompress
}
```