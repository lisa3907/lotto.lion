## dotnet publish

$ sudo cp ~/odinsoft-git/solution-team/lottolion/shells/build.lion.worker.sh /usr/local/bin/.
$ sudo chmod +x /usr/local/bin/build.lion.worker.sh

## supervisor

$ sudo nano /etc/supervisor/conf.d/lion.worker.conf

```
[program:lion.worker]
command=/usr/bin/dotnet /var/lion.worker/lion.worker.dll 
directory=/var/lion.worker/
autostart=true
autorestart=true
stderr_logfile=/var/log/lion.worker.err.log
stdout_logfile=/var/log/lion.worker.out.log
environment=Hosting:Environment=Production
user=www-data
stopsignal=INT
```

## logrotate

$ sudo nano /etc/logrotate.d/lion.worker

```
/var/log/lion.worker.out.log {
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

/var/log/lion.worker.err.log {
        rotate 4
        weekly
        missingok
        create 640 root adm
        notifempty
        compress
        delaycompress
}
```