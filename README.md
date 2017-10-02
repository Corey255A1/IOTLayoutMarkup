# IOTLayoutMarkup
Developing a light weight protocol to layout controls from an embedded device (such as Arduino)

The Client program asks the "Server" for information. The Server then responds back with the controls and control types.
The controls have a position and size with in the client rendering area.
Then the server can send updates to an ID and the client will determine which control (based on ID) needs the information updated in.

The project stems from not wanting to have to constantly design a new front end to each embedded project. It would be nice if the embedded software can just dictate the layout of the controls and the client just present it. Not unlike a Web browser and a web server.
HTML/HTTP seemed overly heavy and character based. Having to send things like <head></head> has a lot of over head.
The current project is using serial communication which is very slow in comparision to ethernet or wifi speeds. Reducing overhead can make the protocol efficient.

Currently prototyping the protocol using an Arduino and C# WPF, however, the end goal is to create a protocol that can be used to communicate between different sorts of embedded computers and different client programming languages.


