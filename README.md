# Hacking on WCAP

This is a project from my stream on Twitch. I've been experimenting with
the Weston Wayland Compositor, particularly it's screen capture features.

The `weston-src/` directory contains a version of Weston that has been 
modified (the quick and dirty way :P) to output its WCAP data over a TCP 
socket. `client/` contains a simple in-progress client written in Crystal
to connect to that socket (with no video output right now, since I got 
distracted...)

### References
[WCAP format](https://cgit.freedesktop.org/wayland/weston/tree/wcap/README)
