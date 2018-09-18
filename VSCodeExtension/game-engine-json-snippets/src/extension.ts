'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
var path = require('path');
var exec = require('child_process').execFile;

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "game-engine-json-snippets" is now active!');

    // The command has been defined in the package.json file
    // Now provide the implementation of the command with  registerCommand
    // The commandId parameter must match the command field in package.json

    let disposable = vscode.commands.registerCommand('extension.updateSnippets', () => {
        // The code you place here will be executed every time your command is executed
        var snippet_parser = "../snippet_parser";
        var exePath = path.resolve(__dirname, snippet_parser+'/VSCodeExtension.exe');
        var settingsPath = path.resolve(__dirname, snippet_parser+'/settings.json');
        var outPath = path.resolve(__dirname, snippet_parser+'/componentTypes.json');

        exec(exePath, [settingsPath, outPath], (err: any, data: any) => {
            console.log(err);
            console.log(data.toString());
        });
        // Display a message box to the user
        vscode.window.showInformationMessage("Successfully updated snippets");
    });

    context.subscriptions.push(disposable);
}

// this method is called when your extension is deactivated
export function deactivate() {
}