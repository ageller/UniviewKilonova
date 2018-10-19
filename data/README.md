cat grid512.obj | grep -v vt | grep -v vn | sed s/"\/"/" "/g | awk '{if ($1 == "f"){print $1,$2,$4,$6}else{print $0}}' > grid512.v2.obj

