# Topaz

**[Early development]**

## Introduction

Topaz consists of two parts:

- The client/server system that runs on the computer.
- The shortcuts that run on your iOS/iPadOS device.

## How Topaz works

Topaz works by running a program on your computer from your iOS/iPadOS device, through SSH, which then triggers a back-and-forth flow of messages between the two devices.

The parts of Topaz that run on your computer consists of two systems:

- A client, which the shortcut invokes, sending the message on to the server.
- A server, which receives input from the client, and passes a response back to the client, which will then be returned to the iOS/iPadOS device via SSH.

The reason for using a client/server system on the computer is twofold:

1. It allows for the client to be very thin, acting as little more than an intermediary between the shortcut and the server, leaving persistent data to be stored by the server. This also gets around the issue of the client, which only exists for the lifetime of a single SSH call, needing to store data between multiple SSH calls happening within a single command.
2. It gets around the issue of SSH running in session 0 on Windows, which means any application launched will also launch in session 0, and would therefore be invisible to the user. Applications like [PsExec](https://docs.microsoft.com/en-us/sysinternals/downloads/psexec) can get around this issue, but it comes with certain complications. Having the server application run on the user's session solves this issue more elegantly.

## Additional technical documentation

### Client

The client will take the first commandline argument (the first string in `static void Main(string[] args)`), and send it to the server. If the string starts with '#', it is expected to be a valid base64 encoded string. If not, it will be convereted to a base64 encoded string by the client before sending it to the server (this is intended for testing and debugging on the computer).
