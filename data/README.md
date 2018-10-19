for some reason, this kind of works, but only links half the triangles ("happy accident"):

cat grid512.obj | sed s/"\/"/" "/g | awk '{if ($1 == "v" || $1 == "f"){if ($1 == "f"){print $1,$2,$4,$6,$8}else{print $0}}}' > grid512.v3.obj


This doesn't work:

cat grid512.obj | sed s/"\/"/" "/g | awk '{if ($1 == "v" || $1 == "f"){if ($1 == "f"){print $1,$2,$5,$8,$11}else{print $0}}}' > grid512.v3.obj

cat grid512.obj | sed s/"\/"/" "/g | awk '{if ($1 == "v" || $1 == "f"){if ($1 == "f"){print $1,$4,$7,$10,$13}else{print $0}}}' > grid512.v3.obj


cat grid512.obj | awk '{if ($1 == "v" || $1 == "f") print$0}' > grid512.v4.obj

