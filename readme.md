# Read Me

This repository houses AWS related 
things for Transformalize.

# What's Available

There's only one thing available right now to just to see how it works.
Prior to anything working you have to assume a role.

## sts get-caller-identity

Currently this is a transform that returns 
a JSON response from the AWS STS (Secure Token Service) 
to get the caller's identity.

You use it as a transform like this:

`t="stsgetcalleridentity()"`
