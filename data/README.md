cat grid512.v2.obj | sed s/"\/"/" "/g | awk '{if ($1 == "f"){print $1,$2,$4,$6, $8}else{print $0}}' > grid512.v3.obj
