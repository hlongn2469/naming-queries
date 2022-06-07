# Names query web application
Create and host a Website which has three sections: 
1) A button call Load Data 
2) A button call Clear Data 
3) Two input text boxes labeled First Name and Last Name 
4) A button called Query 
- [ ] When the “Load Data” button is hit the website will load data from an object stored at a given 
URL.  I will have the data both in Azure as well as Amazon S3.   The CORS (Cross-Origin Resource 
Sharing) header has been added to the bucket so that the object can be accessed from different 
regions.

- [ ] When the “Clear Data” button is hit the blob is removed from the object store.  The NoSQL 
table is also emptied or removed. 
 
- [ ] Once the data has been loaded in the NoSQL store the Website user can type in either one or 
both a First and Last name.  When the Query button is hit results are shown on the Website.  
For the queries the names should be exact matches.  Note that you do not need to fill in both 
query boxes to query. 

# Design Document 
[Program4-designdoc.pdf](https://github.com/hlongn2469/naming-queries/files/8856387/Program4-designdoc.pdf)
