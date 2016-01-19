#!/bin/bash

manifestPath=
placeholderKey=
googleMapsKey=

while getopts "m:p:g:" o; do
	case "${o}" in
		m) manifestPath="${OPTARG}" ;;
		p) placeholderKey="${OPTARG}" ;;
		g) googleMapsKey="${OPTARG}" ;;
	esac
done
shift $((OPTIND-1))

sed -i '' "s/$placeholderKey/$googleMapsKey/g" "$manifestPath"