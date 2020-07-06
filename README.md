
# Actionstep API Demonstration Application

This sample application demonstrates common usage scenarios for interacting with Actionstep's public API. 
It shows how to construct both simple and complex requests and how to interpret the responses.
It also shows how to implement the OAuth authentication protocol.
Further detailed information can be found on the [API Developer Portal](https://docs.actionstep.com/).

## Core Functionality

The application's functionality is divided into five core areas.

### File Notes

The file notes page demonstrates performing read, create, update, and delete operations.
It also demonstrates the use of REST hooks for the file note creation event (assuming such a REST hook has been regsitered on the REST Hooks page).

### Documents

The documents page demonstrates how to both upload and download a document associated with a matter.

### Data Collections and Fields

The data collections and fields page demonstrates how to create a data field collection and the individual data fields within that collection.
It shows how data collections are associated with a matter type, and how to set some of the core properties of a data field.

### Matters (Custom Field Data)

The matters page demonstrates how custom data fields can be used to attach additional data items to a matter, using the data collections and fields
created using the data collections and fields page.

### REST Hooks

The REST hooks page demonstrates how to register a web hook for an Actionstep event (the FileNoteCreated event) and how to respond to received callbacks.

### Technology Stack

The application is built using Microsoft .NET Core and Blazor (the new Web Assembly technology for building web applications in native languages).
To build and run the application you will need to install [Microsoft Visual Studio (Community Preview Edition)](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=community&ch=pre&rel=16) which is free to download and install.
The source code includes a number of third-party open source components from Github, for example, Newtonsoft Json, and the Polly retry library.

#### Ngrok

In order to demonstrate the use of OAuth and REST (web) hooks from your local development environment we recommend using [Ngrok](https://ngrok.com/) to proxy 
incoming responses back to your localhost environment.  

## MIT License

Copyright 2020 Actionstep Limited

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
