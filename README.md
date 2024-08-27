# VSCRunner
This is an application created for my school to save and load Visual Studio Code extensions, as the school computers reset after each restart.

This project started as a batch script made by another student in my class and was expanded upon by me before being converted to a C# program due to the advantages it provided.

## Features
- Save VS Code extensions and settings to a separate directory, and load them from it.
- Output VS Code logs and error messages in a console window.

## Planned Features
- Multi-threading to improve loading times.
- Dynamic directory paths, as they are currently hardcoded for our school's system.
- Checks to avoid copying files that don't need to be copied. (aka ones that already exist and are exactly the same)
- Checks to prevent saving duplicate extensions, since the version of Visual Studio Code on our school PCs does not seem to delete/replace old versions when updating them.

## Contributing

Feel free to help! <a href="https://github.com/NineOfGaming/VSCRunner/issues/new">Open an issue</a> or submit PRs.

## License

This project is licensed under the GNU General Public License v3.0 - see the <a href="./LICENSE">LICENSE</a> file for details.
