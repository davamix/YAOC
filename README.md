# YAOC
Yet Another Ollama Client. 

This is a desktop application written in C# and WPF to play with your local models.

## Features
* Attach files to the conversation
* Plugin system to allow parse different types of files (currently only .txt files)

## Requirements
* [Ollama client](https://ollama.com/)

* ~~Ollama server running on 127.0.0.1:11434 (default)~~ Can be changed in the settings
* Download the models via Ollama commands
	* ollama pull your-model

## Setup
* <b>Yaoc</b> is the main project. This will create a <i>Plugins</i> folder on the output directory via post-build event.
* <b>Yaoc.Plugins.PlainText</b> contains the plugin to extract content from <i>.txt</i> files.
	* Build or Publish this project and copy the output <i>.dll</i> on <b>Yaoc</b>'s <i>Plugins</i> folder.
