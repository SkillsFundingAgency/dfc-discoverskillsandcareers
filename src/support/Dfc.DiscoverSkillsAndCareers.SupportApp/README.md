A little utility app to help with various support operational tasks against the discover my skills service.

# Usage

## Loading Questions 

    dotnet run -- load-questions -f path/to/csv/file -v versionkey

For example:

    dotnet run -- load-questions -f "C:\ncs\dfc-discoverskillsandcareers-dev\data\short_assessment_statements.csv" -v 201901

## Creating Validity Test Sessions 

**Note:** For this to work at least 1 question set must be loaded. 


    dotnet run -- create-validity-sessions -n 300

For example:

    dotnet run -- create-validity-sessions -n 300