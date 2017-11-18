# ShareFile "FileSharer"
A sample application to play with AWS

## Main components 
* Highly available web service that listens to ShareFile events configured via webhook. For every file uploaded, it will create a Share link, create a message containing the link and some other metadata. And post it on an SNS topic
* SNS topic that will place messages in a queue, and also send an email to a recipient,
* The queue will be monitored by another instance that will replicate the file in S3, and then create a record in Dynamo with some file metadata.

