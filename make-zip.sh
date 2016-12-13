#!/bin/bash
set -e
cd "$(dirname "$0")"
zip "Distrib-$(date +"%Y-%m-%d").zip" \
	Distrib/*.air \
	Distrib/*.unityPackage \
	Distrib/*.zxp \
	Distrib/html/* \
