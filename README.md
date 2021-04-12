# nextmasjid-backend
NextMasjid Core project for reading data, and API project.

# CI CD
The CICD.yml file is the workflow file for publishing the api project to azure app service.
The Data folder that contains the data to be loaded is now currently checked into the repo.
When the CICD workflow runs, this data folder is copied into app service (for this, please set to the compile action to *Content* and the build action to *copy always* for each of the files present in the data folder
