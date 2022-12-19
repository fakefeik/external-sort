# external-sort

### Basic usage

```bash
# build solution
dotnet build
  
# generate approx 5GB file
./ExternalSort.Generator/bin/net7.0/ExternalSort.Generator.exe --lines-count 100000000
  
# sort generated file
./ExternalSort/bin/net7.0/ExternalSort.exe --input gen.txt --output sorted.txt
```

with default parameters sorting ExternalSort uses ~2.5GB of RAM & all available CPU when sorting large files.

generator & sorter parameters can be tuned, see `./ExternalSort.Generator.exe -h` or `./ExternalSort.exe -h` to learn more.