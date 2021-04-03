# CsvToPtt
Parse and format CSV file to paste to PTT via PCMan

Example input 1: 
````
CsvToPtt.exe "input.csv"
````
This input will parse input.csv and output to default path output.txt

````
Example input 2: CsvToPtt.exe "input.csv" "myOutputPath.txt"
````
This input will parse input.csv and output to myOutputPath.txt

Example:
input
````
Timestamp,2021/03/04 07:54:56,2021/03/05 08:48:45,2021/03/06 08:13:04
杏仁ミル,31247,29692,29614
香草奈若,14020,13585,13499
鳥羽樂奈,11006,10952,10969
如月ルミィ,2712,2699,2792
````

output
````
Timestamp   2021/03/04 07:54:56  2021/03/05 08:48:45  2021/03/06 08:13:04
杏仁ミル                  31247                29692                29614
香草奈若                  14020                13585                13499
鳥羽樂奈                  11006                10952                10969
如月ルミィ                 2712                 2699                 2792
````
This looks great on PTT I swear.
