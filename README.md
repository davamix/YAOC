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

<img width="960" alt="YAOC_1" src="https://github.com/user-attachments/assets/6db29675-f3b4-4b92-b886-e21ee187ab7c" />

<img width="960" alt="YAOC_2" src="https://github.com/user-attachments/assets/0ed18254-6a29-41bf-a012-fdeadecb8806" />

<img width="960" alt="YAOC_3" src="https://github.com/user-attachments/assets/3e5ff486-7d93-4bfe-853b-050345300d6d" />

<img width="960" alt="YAOC_4" src="https://github.com/user-attachments/assets/bede94c2-afb7-4c0c-b82b-742286daa016" />
