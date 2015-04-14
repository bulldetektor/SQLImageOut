SQLImageOut
===========

Extracts binary/jpg data from SQLServer database from a simple query

--------------------------------------------------

    Usage:
    SQLImageOut "[connection string]" "[query]"
    
    e.g.
    To extact all images from table Image into separate files 1.jpg, 2.jpg, etc.
    SQLImageOut "Provider=SQLOLEDB;Data Source=(local);..." "SELECT CONVERT(varchar(50), ImageID)+'.jpg', ImageData from Image"
    or        
    SQLImageOut @ConnectionString.txt @Query.sql

---------------------------------------------------

Copyright 2012 Andrew Joiner

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
