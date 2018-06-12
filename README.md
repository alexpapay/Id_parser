# Id_parser
Console data parser from vk.com with a different types of parsing. Designed to bypass the vk.com token lock algorithm by timeouts and tokens/proxy servers lists.

# The following parsing algorithms were designed and tested:
1. Assembling with sequential search of tokens during blocking (the token works before blocking);
2. Build with a random token from the list for each request;
3. Build with a random date of birth (within the range) and a random token from the list. The most workable way.
4. Workpiece to work with the execute method from vk.api to work with random tokens and proxy servers.

# Notes
There is support for the list of auth tokens  and ipv6 proxy servers.
In each method, checking tokens to block and exclude a banned token from the list (+ restart the entire list again if the token is diluted if the ban was short-term).
