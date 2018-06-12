# Id_parser
Data parser from vk.com with different parsing algorithms.

# The following parsing algorithms were designed and tested:
1. Assembling with sequential search of tokens during blocking (the token works before blocking);
2. Build with a random token from the list for each request;
3. Build with a random date of birth (within the range) and a random token from the list. The most workable way.
4. Workpiece to work with the execute method from vk.api to work with random tokens and proxy servers.

# Notes
There is support for the list of tokens and ipv6 proxy servers.
In each method, checking tokens to block and exclude a banned token from the list (+ restart the entire list again if the token is diluted if the ban was short-term).
