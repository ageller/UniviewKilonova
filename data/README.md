this kind of worked in U2, but had some strange artifacts:


cat grid512.obj | sed s/"\/"/" "/g | awk '{if (NR > 4){if ($1 == "f"){print $1,$2,$4,$6,$8}else{print $0}}}' > grid512.v3.obj


