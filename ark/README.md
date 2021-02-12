#Ark!

---
Ark is a program I wrote to deal with the difficulty of decompressing and compressing on the command line, with the various tools that exist.
They all take different arguments and almost none of them are universal decompressors.

Ark aims to change that by supporting a large variety of compression schemes for decompression, and some of the most common ones for compression.

Usage:
````
 -d, --decompress     (Default: false) Specify for Decompression.

  -c, --compress       (Default: false) Specify for Compressing.

  -i, --input          Required. Input files to be processed.

  -o, --output         (Default: ./) Output Location. Defaults to ./output.zip when compressing.

  -s, --store-paths    (Default: false) Stores Compressed files with the full path that they had on
                       disk. Only useful during compression.

  -r, --recursive      (Default: false) Recurse Directories when Compressing

  --verbose            (Default: false) Prints all messages to standard output.

  --help               Display this help screen.

  --version            Display version information.
````
Example:
````
./ark -i ./ -o ./out.zip -c --verbose -r
Updating File List Recursively...
Getting Files...
Created MemoryStream
Adding ./ark.deps.json
Adding ./CommandLine.dll
Adding ./ark.runtimeconfig.dev.json
Adding ./SharpCompress.dll
Adding ./ark.runtimeconfig.json
Adding ./ark
Adding ./ark.pdb
Adding ./ark.dll
Adding ./ref/ark.dll
Compressing...
Writing to Disk...
Done!
````