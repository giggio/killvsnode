# Kill VS Node

A DNX based command line utility to kill the annoying Visual Studio node process spawn by
Visual Studio.

For more information see issue [#111 on aspnet/tooling](https://github.com/aspnet/Tooling/issues/111).

## Installing

Simply run:

```powershell
dnu commands install killvsnode
```

## Running

Simply run:

````powershell
killvsnode
````

If you want to know what processes are being killed, run:

````powershell
killvsnode --verbose
````

To stop press CTRL-C.

## Maintainer

* [Giovanni Bassi](http://blog.lambda3.com.br/L3/giovannibassi/), aka Giggio, [Lambda3](http://www.lambda3.com.br), [@giovannibassi](http://twitter.com/giovannibassi)

Contributors can be found at the [contributors](https://github.com/giggio/killvsnode/graphs/contributors) page on Github.

## Contact

I am only on Jabbr most of the day, usually on the [ASP.NET vNext room](https://jabbr.net/#/rooms/AspNetvNext), with user name `Giggio`.

## License

This software is open source, licensed under the Apache License, Version 2.0.
See [LICENSE.txt](https://github.com/giggio/killvsnode/blob/master/LICENSE.txt) for details.
Check out the terms of the license before you contribute, fork, copy or do anything
with the code. If you decide to contribute you agree to grant copyright of all your contribution to this project, and agree to
mention clearly if do not agree to these terms. Your work will be licensed with the project at Apache V2, along the rest of the code.