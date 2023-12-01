# FileSystemDemo

search-file-system
    - use "file" to search for file across the file system
    - use "folder/file" to search for file within specific folder
    - returns "/" when no results were found

create-file-or-folder
    - "file", "/file" will create file in root directory
    - "folder/", "/folder/" will create folder in root
    - "folder/folder1/file" will create file within folder1 within folder
    - returns "/" when no action was performed

delete-file-or-folder
    - "file" removes file on root directory
    - "folder/" removes directory and all its children
    - "folder/file" removes file from folder
    - returns "/" when no action was performed