# IOTLayoutMarkup
Developing a light weight protocol to layout controls from an embedded device (such as Arduino)

The Client program asks the "Server" for information. The Server then responds back with the controls and control types.
The controls have a position and size with in the client rendering area.
Then the server can send updates to an ID and the client will determine which control (based on ID) needs the information updated in.

