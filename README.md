
<!-- ABOUT THE PROJECT -->
## Contract Portal
<br>

### Prerequisites
<hr>
Install docker.
<br>
<br>


### Run the app locally
<hr>
Run the following command in the root directory of this project

```bash
docker build -t contract-portal .
docker run -d -p 8080:80 --name contract-portal -t contract-portal
```

Now you can access the web portal at http://localhost:8080/

To check the binary files uploaded, run the following command
```bash
sudo docker exec -it contract-portal /bin/bash
```
through which you can shell into the running container. Currently binary files are stored at <i>/tmp</i> directory.

Run
```bash
ls /tmp
```
and you shall see previously uploaded binaries.
### Check the app on GCP 
When 'allUsers' is granted with 'Cloud Run Invoker', the app can be accessed at: 
https://contract-portal-web-emwnlmwkea-uw.a.run.app/
