# JPM Analysis - Backend

```txt
$: dotnet add package MySql.EntityFrameworkCore --version 8.0.0
```

```txt
$: dotnet tool install --global dotnet-ef
```

```txt
$: dotnet add package Microsoft.EntityFrameworkCore.Tools
```

```txt
$: dotnet add package Microsoft.EntityFrameworkCore.Design
```

```txt
$: dotnet restore
```

```txt
$: docker compose up -d
```

```txt
$: dotnet ef dbcontext scaffold "Server=localhost;Uid=root;Pwd=1234;Database=jpm_analysis_database" MySql.EntityFrameworkCore -o Models
```

```txt
$: dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

```txt
$: dotnet add package OfficeOpenXml.Core.ExcelPackage
```

```txt
$: dotnet add package EPPlus
```

```txt
$:  dotnet add package Newtonsoft.Json
```
