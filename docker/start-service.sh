#!/usr/bin/bash

set -e

cd /opt/backend/
./NextMasjid.Backend.API --urls http://0.0.0.0:5000
