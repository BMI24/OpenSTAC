using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class AtomSDFComponent : ComponentBase, IColorSDFComponent
    {
        public record struct AtomData(Vector3 Coords, float VDWRadius, Vector3 ElementColor, float BFactor);

        public List<AtomData> Atoms { get; set; } = new List<AtomData>();

        public int AtomCount
        {
            get => Atoms.Count;
            set => LoadAtomsFromData(value);
        }
        int K = 15;
        public int UsedAtoms { get; set; }
        public float RProbe { get; set; }

        public GlobalIdentifier AtomStructN { get; } = "atom";
        public GlobalIdentifier SDSphereFN { get; } = "sdSphere";
        public GlobalIdentifier SDOuterFN { get; } = "sdOuter";
        public GlobalIdentifier TwoSortFN { get; } = "two_sort";
        public GlobalIdentifier InTriangleFN { get; } = "in_triangle";
        public GlobalIdentifier SortElementsFN { get; } = "sort_elements";
        public GlobalIdentifier Case2FN { get; } = "case2";
        public GlobalIdentifier Case3FN { get; } = "case3";
        public GlobalIdentifier SDFName { get; } = "ses";
        public GlobalIdentifier ColorOutputName { get; } = "atomColor";
        public bool IsColorSDF { get; set; }

        public void LoadAtomsFromData(int atomCount)
        {
            var rows = Data.Split(Environment.NewLine);
            atomCount = Math.Min(atomCount, rows.Length);
            List<AtomData> atoms = new();
            for (int i = 0; i < atomCount; i++)
            {
                var r = rows[i].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                atoms.Add(new(
                    new(float.Parse(r[0]), float.Parse(r[1]), float.Parse(r[2])),
                    float.Parse(r[3]),
                    new(float.Parse(r[4]), float.Parse(r[5]), float.Parse(r[6])),
                    float.Parse(r[7])));
            }

            Atoms = atoms;
        }

        private string Data = """
            -2.4235 3.382 3.382 1.55 0 0 1 0.307296
            -2.6225 2.628 2.628 1.7 0.2 0.2 0.2 0.288182
            -1.5945 1.526 1.526 1.7 0.2 0.2 0.2 0.25896
            -0.8435 1.189 1.189 1.52 1 0 0 0.226061
            -1.5715 0.953001 0.953001 1.55 0 0 1 0.265392
            -0.6225 -0.101 -0.101 1.7 0.2 0.2 0.2 0.223672
            -0.7375 -1.321 -1.321 1.7 0.2 0.2 0.2 0.24977
            0.2595 -1.966 -1.966 1.52 1 0 0 0.266863
            -0.8055 -0.539999 -0.539999 1.7 0.2 0.2 0.2 0.192795
            0.3125 -1.507 -1.507 1.7 0.2 0.2 0.2 0.222753
            -2.1805 -1.17 -1.17 1.7 0.2 0.2 0.2 0.209337
            0.3055 -1.852 -1.852 1.7 0.2 0.2 0.2 0.136923
            -1.9515 -1.636 -1.636 1.55 0 0 1 0.272744
            -2.1535 -2.794 -2.794 1.7 0.2 0.2 0.2 0.284874
            -1.4265 -2.678 -2.678 1.7 0.2 0.2 0.2 0.292593
            -0.7485 -3.614 -3.614 1.52 1 0 0 0.276971
            -3.6555 -3.043 -3.043 1.7 0.2 0.2 0.2 0.288918
            -3.8425 -4.156 -4.156 1.7 0.2 0.2 0.2 0.288734
            -4.3325 -3.433 -3.433 1.7 0.2 0.2 0.2 0.249403
            -1.5565 -1.536 -1.536 1.55 0 0 1 0.319978
            -0.8905 -1.355 -1.355 1.7 0.2 0.2 0.2 0.345341
            0.6275 -1.319 -1.319 1.7 0.2 0.2 0.2 0.33707
            1.3565 -1.844 -1.844 1.52 1 0 0 0.296085
            -1.3595 -0.0649986 -0.0649986 1.7 0.2 0.2 0.2 0.39331
            -0.6835 0.200001 0.200001 1.7 0.2 0.2 0.2 0.512406
            -0.9225 -0.913 -0.913 1.7 0.2 0.2 0.2 0.583716
            -2.1005 -1.192 -1.192 1.52 1 0 0 0.654291
            0.0675 -1.511 -1.511 1.52 1 0 0 0.642897
            1.1005 -0.705999 -0.705999 1.55 0 0 1 0.316854
            2.5315 -0.6 -0.6 1.7 0.2 0.2 0.2 0.340195
            3.2075 -1.851 -1.851 1.7 0.2 0.2 0.2 0.329167
            4.3325 -2.166 -2.166 1.52 1 0 0 0.320897
            2.8275 0.573 0.573 1.7 0.2 0.2 0.2 0.378607
            3.0335 1.905 1.905 1.7 0.2 0.2 0.2 0.48741
            1.9265 2.227 2.227 1.7 0.2 0.2 0.2 0.565705
            3.5305 2.983 2.983 1.7 0.2 0.2 0.2 0.565705
            -1.2515 2.066 2.066 1.52 1 0 0 0.623599
            -1.3545 2.768 2.768 1.52 1 0 0 0.623599
            -0.6995 2.684 2.684 1.55 0 0 1 0.576181
            -1.1705 4.156 4.156 1.55 0 0 1 0.576181
            4.009 14.772 14.772 1.55 0 0 1 0.274214
            4.635 13.613 13.613 1.7 0.2 0.2 0.2 0.277523
            4.163 12.232 12.232 1.7 0.2 0.2 0.2 0.222937
            4.852 11.243 11.243 1.52 1 0 0 0.259511
            4.531 13.739 13.739 1.7 0.2 0.2 0.2 0.267598
            5.179 15.31 15.31 1.8 1 1 0 0.280463
            2.996 12.157 12.157 1.55 0 0 1 0.2551
            2.458 10.877 10.877 1.7 0.2 0.2 0.2 0.251792
            2.574 10.732 10.732 1.7 0.2 0.2 0.2 0.250505
            3.05 9.71 9.71 1.52 1 0 0 0.237273
            0.998 10.73 10.73 1.7 0.2 0.2 0.2 0.250322
            0.108 9.364 9.364 1.8 1 1 0 0.285977
            2.144 11.753 11.753 1.55 0 0 1 0.261717
            2.233 11.708 11.708 1.7 0.2 0.2 0.2 0.313178
            3.696 11.881 11.881 1.7 0.2 0.2 0.2 0.324205
            4.156 11.341 11.341 1.52 1 0 0 0.351038
            1.355 12.798 12.798 1.7 0.2 0.2 0.2 0.275133
            -0.106 12.599 12.599 1.7 0.2 0.2 0.2 0.285242
            -1.066 13.503 13.503 1.55 0 0 1 0.230472
            -0.77 11.596 11.596 1.7 0.2 0.2 0.2 0.295902
            -2.258 13.065 13.065 1.7 0.2 0.2 0.2 0.308399
            -2.106 11.91 11.91 1.55 0 0 1 0.295902
            4.422 12.636 12.636 1.55 0 0 1 0.346811
            5.851 12.858 12.858 1.7 0.2 0.2 0.2 0.37438
            6.507 12.493 12.493 1.7 0.2 0.2 0.2 0.36905
            5.812 12.118 12.118 1.52 1 0 0 0.377688
            6.132 14.318 14.318 1.7 0.2 0.2 0.2 0.40544
            5.568 14.63 14.63 1.52 1 0 0 0.504319
            7.829 12.595 12.595 1.55 0 0 1 0.356552
            8.528 12.239 12.239 1.7 0.2 0.2 0.2 0.354714
            9.018 13.438 13.438 1.7 0.2 0.2 0.2 0.351222
            9.581 14.382 14.382 1.52 1 0 0 0.346811
            9.725 11.315 11.315 1.7 0.2 0.2 0.2 0.384304
            9.209 9.946 9.946 1.7 0.2 0.2 0.2 0.403051
            10.626 11.177 11.177 1.7 0.2 0.2 0.2 0.381731
            10.304 8.98 8.98 1.7 0.2 0.2 0.2 0.452307
            8.798 13.391 13.391 1.55 0 0 1 0.315383
            9.226 14.47 14.47 1.7 0.2 0.2 0.2 0.345341
            10.42 14.111 14.111 1.7 0.2 0.2 0.2 0.320713
            10.554 12.979 12.979 1.52 1 0 0 0.329167
            8.111 14.883 14.883 1.7 0.2 0.2 0.2 0.31281
            6.716 15.768 15.768 1.8 1 1 0 0.375666
            11.268 15.07 15.07 1.55 0 0 1 0.305091
            12.412 14.948 14.948 1.7 0.2 0.2 0.2 0.325859
            11.885 15.159 15.159 1.7 0.2 0.2 0.2 0.317772
            10.741 15.595 15.595 1.52 1 0 0 0.324756
            13.468 15.999 15.999 1.7 0.2 0.2 0.2 0.362801
            12.966 17.298 17.298 1.52 1 0 0 0.344973
            12.687 14.87 14.87 1.55 0 0 1 0.317772
            12.259 15.066 15.066 1.7 0.2 0.2 0.2 0.318691
            11.86 16.527 16.527 1.7 0.2 0.2 0.2 0.30454
            10.844 16.813 16.813 1.52 1 0 0 0.299945
            13.396 14.678 14.678 1.7 0.2 0.2 0.2 0.329351
            13.213 14.919 14.919 1.7 0.2 0.2 0.2 0.35453
            12.081 14.061 14.061 1.7 0.2 0.2 0.2 0.354898
            14.514 14.591 14.591 1.7 0.2 0.2 0.2 0.374564
            12.658 17.448 17.448 1.55 0 0 1 0.294431
            12.397 18.879 18.879 1.7 0.2 0.2 0.2 0.326411
            11.081 19.279 19.279 1.7 0.2 0.2 0.2 0.30454
            10.375 20.171 20.171 1.52 1 0 0 0.271274
            13.567 19.667 19.667 1.7 0.2 0.2 0.2 0.417754
            14.896 19.113 19.113 1.7 0.2 0.2 0.2 0.498805
            15.262 19.156 19.156 1.7 0.2 0.2 0.2 0.531704
            15.756 18.478 18.478 1.7 0.2 0.2 0.2 0.525639
            16.446 18.576 18.576 1.7 0.2 0.2 0.2 0.575262
            16.946 17.893 17.893 1.7 0.2 0.2 0.2 0.575262
            17.282 17.946 17.946 1.7 0.2 0.2 0.2 0.600257
            18.45 17.36 17.36 1.52 1 0 0 0.642529
            10.757 18.603 18.603 1.55 0 0 1 0.316486
            9.527 18.852 18.852 1.7 0.2 0.2 0.2 0.301783
            8.34 18.407 18.407 1.7 0.2 0.2 0.2 0.310788
            7.304 19.072 19.072 1.52 1 0 0 0.268701
            9.553 18.052 18.052 1.7 0.2 0.2 0.2 0.364455
            9.106 18.815 18.815 1.7 0.2 0.2 0.2 0.450652
            10.061 18.621 18.621 1.7 0.2 0.2 0.2 0.469767
            10.564 17.521 17.521 1.52 1 0 0 0.451571
            10.311 19.689 19.689 1.55 0 0 1 0.523617
            8.502 17.269 17.269 1.55 0 0 1 0.254181
            7.451 16.734 16.734 1.7 0.2 0.2 0.2 0.268701
            7.216 17.652 17.652 1.7 0.2 0.2 0.2 0.267965
            6.078 17.821 17.821 1.52 1 0 0 0.252711
            7.836 15.328 15.328 1.7 0.2 0.2 0.2 0.294247
            6.72 14.312 14.312 1.7 0.2 0.2 0.2 0.364455
            5.92 14.092 14.092 1.7 0.2 0.2 0.2 0.336519
            7.321 12.993 12.993 1.7 0.2 0.2 0.2 0.396618
            8.289 18.25 18.25 1.55 0 0 1 0.251424
            8.172 19.139 19.139 1.7 0.2 0.2 0.2 0.26043
            7.37 20.407 20.407 1.7 0.2 0.2 0.2 0.250873
            7.003 21.133 21.133 1.52 1 0 0 0.22937
            9.556 19.529 19.529 1.7 0.2 0.2 0.2 0.313729
            10.355 18.377 18.377 1.7 0.2 0.2 0.2 0.396802
            11.635 18.837 18.837 1.7 0.2 0.2 0.2 0.455615
            12.652 18.993 18.993 1.52 1 0 0 0.525271
            11.623 19.056 19.056 1.52 1 0 0 0.472523
            7.1 20.679 20.679 1.55 0 0 1 0.208601
            6.315 21.854 21.854 1.7 0.2 0.2 0.2 0.219996
            4.874 21.663 21.663 1.7 0.2 0.2 0.2 0.222569
            4.099 22.613 22.613 1.52 1 0 0 0.23084
            6.322 22.063 22.063 1.7 0.2 0.2 0.2 0.262268
            7.58 22.743 22.743 1.7 0.2 0.2 0.2 0.272193
            7.901 22.673 22.673 1.52 1 0 0 0.35986
            8.289 23.42 23.42 1.55 0 0 1 0.30987
            4.508 20.429 20.429 1.55 0 0 1 0.212461
            3.145 20.165 20.165 1.7 0.2 0.2 0.2 0.186363
            2.987 19.954 19.954 1.7 0.2 0.2 0.2 0.190406
            1.931 19.523 19.523 1.52 1 0 0 0.180849
            2.581 18.985 18.985 1.7 0.2 0.2 0.2 0.207866
            2.628 19.248 19.248 1.7 0.2 0.2 0.2 0.208969
            1.742 20.145 20.145 1.7 0.2 0.2 0.2 0.22698
            3.591 18.641 18.641 1.7 0.2 0.2 0.2 0.238008
            1.825 20.426 20.426 1.7 0.2 0.2 0.2 0.243889
            3.681 18.914 18.914 1.7 0.2 0.2 0.2 0.273663
            2.799 19.804 19.804 1.7 0.2 0.2 0.2 0.241684
            2.906 20.052 20.052 1.52 1 0 0 0.306745
            4.034 20.273 20.273 1.55 0 0 1 0.22018
            3.975 20.166 20.166 1.7 0.2 0.2 0.2 0.239662
            3.376 21.474 21.474 1.7 0.2 0.2 0.2 0.275317
            3.44 22.484 22.484 1.52 1 0 0 0.325675
            5.365 20.041 20.041 1.7 0.2 0.2 0.2 0.209704
            6.309 18.544 18.544 1.8 1 1 0 0.219445
            2.812 21.465 21.465 1.55 0 0 1 0.309318
            2.228 22.675 22.675 1.7 0.2 0.2 0.2 0.329719
            3.332 23.582 23.582 1.7 0.2 0.2 0.2 0.347546
            4.493 23.124 23.124 1.52 1 0 0 0.354347
            1.266 22.34 22.34 1.7 0.2 0.2 0.2 0.340562
            0.021 21.599 21.599 1.7 0.2 0.2 0.2 0.319978
            -0.564 21.915 21.915 1.52 1 0 0 0.342033
            -0.406 20.621 20.621 1.55 0 0 1 0.327697
            3.016 24.742 24.742 1.52 1 0 0 0.399743
            16.492 9.449 9.449 1.55 0 0 1 0.358022
            15.024 9.193 9.193 1.7 0.2 0.2 0.2 0.380812
            14.675 8.498 8.498 1.7 0.2 0.2 0.2 0.401948
            15.366 8.658 8.658 1.52 1 0 0 0.376218
            14.249 10.513 10.513 1.7 0.2 0.2 0.2 0.359125
            12.819 10.345 10.345 1.7 0.2 0.2 0.2 0.373645
            12.513 10.139 10.139 1.7 0.2 0.2 0.2 0.383753
            11.781 10.385 10.385 1.7 0.2 0.2 0.2 0.383569
            11.189 9.977 9.977 1.7 0.2 0.2 0.2 0.39478
            10.458 10.225 10.225 1.7 0.2 0.2 0.2 0.371807
            10.161 10.021 10.021 1.7 0.2 0.2 0.2 0.39478
            13.595 7.728 7.728 1.55 0 0 1 0.392207
            13.143 6.994 6.994 1.7 0.2 0.2 0.2 0.430252
            12.933 7.911 7.911 1.7 0.2 0.2 0.2 0.433009
            12.535 9.067 9.067 1.52 1 0 0 0.451755
            11.828 6.249 6.249 1.7 0.2 0.2 0.2 0.45249
            10.719 7.249 7.249 1.7 0.2 0.2 0.2 0.455799
            11.459 5.345 5.345 1.7 0.2 0.2 0.2 0.499908
            13.22 7.384 7.384 1.55 0 0 1 0.437052
            13.064 8.124 8.124 1.7 0.2 0.2 0.2 0.417203
            12.031 7.367 7.367 1.7 0.2 0.2 0.2 0.412424
            12.356 6.75 6.75 1.52 1 0 0 0.394229
            14.403 8.184 8.184 1.7 0.2 0.2 0.2 0.462047
            14.342 9.044 9.044 1.7 0.2 0.2 0.2 0.493843
            14.023 10.233 10.233 1.52 1 0 0 0.539239
            14.654 8.446 8.446 1.55 0 0 1 0.503216
            10.782 7.416 7.416 1.55 0 0 1 0.378607
            9.701 6.711 6.711 1.7 0.2 0.2 0.2 0.368315
            8.444 7.565 7.565 1.7 0.2 0.2 0.2 0.348098
            8.324 8.611 8.611 1.52 1 0 0 0.299761
            9.342 5.437 5.437 1.7 0.2 0.2 0.2 0.414446
            10.472 4.434 4.434 1.7 0.2 0.2 0.2 0.441095
            10.803 3.625 3.625 1.7 0.2 0.2 0.2 0.469767
            9.911 3.188 3.188 1.52 1 0 0 0.50193
            12.092 3.4 3.4 1.55 0 0 1 0.478772
            7.539 7.135 7.135 1.55 0 0 1 0.325859
            6.232 7.782 7.782 1.7 0.2 0.2 0.2 0.305826
            5.43 7.384 7.384 1.7 0.2 0.2 0.2 0.29921
            5.211 6.195 6.195 1.52 1 0 0 0.312259
            5.496 7.281 7.281 1.7 0.2 0.2 0.2 0.315567
            6.114 7.873 7.873 1.7 0.2 0.2 0.2 0.331005
            5.79 9.128 9.128 1.55 0 0 1 0.367028
            7.016 7.345 7.345 1.7 0.2 0.2 0.2 0.355082
            6.48 9.333 9.333 1.7 0.2 0.2 0.2 0.33174
            7.217 8.277 8.277 1.55 0 0 1 0.387796
            5.005 8.381 8.381 1.55 0 0 1 0.257306
            4.255 8.113 8.113 1.7 0.2 0.2 0.2 0.241132
            2.798 8.495 8.495 1.7 0.2 0.2 0.2 0.208418
            2.475 9.669 9.669 1.52 1 0 0 0.17313
            4.859 8.905 8.905 1.7 0.2 0.2 0.2 0.250322
            6.299 8.526 8.526 1.7 0.2 0.2 0.2 0.278258
            6.862 9.51 9.51 1.7 0.2 0.2 0.2 0.23911
            6.321 7.117 7.117 1.7 0.2 0.2 0.2 0.30748
            1.917 7.501 7.501 1.55 0 0 1 0.233045
            0.498 7.748 7.748 1.7 0.2 0.2 0.2 0.235618
            -0.439 7.222 7.222 1.7 0.2 0.2 0.2 0.23764
            -0.121 6.275 6.275 1.52 1 0 0 0.235802
            0.078 7.144 7.144 1.7 0.2 0.2 0.2 0.253262
            1.056 7.675 7.675 1.8 1 1 0 0.262636
            -1.605 7.853 7.853 1.55 0 0 1 0.250873
            -2.621 7.45 7.45 1.7 0.2 0.2 0.2 0.248851
            -2.197 7.392 7.392 1.7 0.2 0.2 0.2 0.243705
            -1.582 8.326 8.326 1.52 1 0 0 0.247013
            -2.527 6.289 6.289 1.55 0 0 1 0.23764
            -2.185 6.149 6.149 1.7 0.2 0.2 0.2 0.210439
            -0.674 6.116 6.116 1.7 0.2 0.2 0.2 0.19886
            -0.188 6.39 6.39 1.52 1 0 0 0.198125
            -2.829 4.891 4.891 1.7 0.2 0.2 0.2 0.25896
            -2.402 3.728 3.728 1.52 1 0 0 0.30601
            0.068 5.779 5.779 1.55 0 0 1 0.192244
            1.52 5.74 5.74 1.7 0.2 0.2 0.2 0.187098
            2.094 7.142 7.142 1.7 0.2 0.2 0.2 0.189487
            3.094 7.373 7.373 1.52 1 0 0 0.231759
            2.13 5.001 5.001 1.7 0.2 0.2 0.2 0.216321
            1.61 3.611 3.611 1.7 0.2 0.2 0.2 0.228634
            1.649 2.687 2.687 1.55 0 0 1 0.240213
            1.031 2.989 2.989 1.7 0.2 0.2 0.2 0.194082
            1.119 1.552 1.552 1.7 0.2 0.2 0.2 0.246094
            0.738 1.709 1.709 1.55 0 0 1 0.220364
            1.46 8.073 8.073 1.55 0 0 1 0.206947
            1.907 9.454 9.454 1.7 0.2 0.2 0.2 0.192244
            1.573 10.063 10.063 1.7 0.2 0.2 0.2 0.178827
            2.332 10.868 10.868 1.52 1 0 0 0.177541
            1.201 10.224 10.224 1.7 0.2 0.2 0.2 0.191877
            1.423 11.738 11.738 1.7 0.2 0.2 0.2 0.174233
            2.912 12.032 12.032 1.7 0.2 0.2 0.2 0.163205
            0.675 12.315 12.315 1.7 0.2 0.2 0.2 0.204926
            0.435 9.662 9.662 1.55 0 0 1 0.179746
            0.019 10.18 10.18 1.7 0.2 0.2 0.2 0.17993
            0.958 9.722 9.722 1.7 0.2 0.2 0.2 0.174968
            1.323 10.509 10.509 1.52 1 0 0 0.182503
            -1.429 9.756 9.756 1.7 0.2 0.2 0.2 0.227348
            -1.788 10.127 10.127 1.7 0.2 0.2 0.2 0.228083
            -2.369 10.438 10.438 1.7 0.2 0.2 0.2 0.209888
            1.361 8.456 8.456 1.55 0 0 1 0.191325
            2.268 7.949 7.949 1.7 0.2 0.2 0.2 0.237089
            3.619 8.646 8.646 1.7 0.2 0.2 0.2 0.225878
            4.26 8.969 8.969 1.52 1 0 0 0.222018
            2.426 6.428 6.428 1.7 0.2 0.2 0.2 0.286344
            3.471 5.813 5.813 1.7 0.2 0.2 0.2 0.368866
            3.375 6.315 6.315 1.7 0.2 0.2 0.2 0.465356
            2.327 6.093 6.093 1.52 1 0 0 0.419224
            4.359 6.935 6.935 1.52 1 0 0 0.491454
            4.048 8.887 8.887 1.55 0 0 1 0.203271
            5.317 9.567 9.567 1.7 0.2 0.2 0.2 0.201985
            5.268 10.939 10.939 1.7 0.2 0.2 0.2 0.191509
            6.221 11.34 11.34 1.52 1 0 0 0.168903
            5.572 9.722 9.722 1.7 0.2 0.2 0.2 0.199779
            4.155 11.653 11.653 1.55 0 0 1 0.1678
            4 12.972 12.972 1.7 0.2 0.2 0.2 0.200882
            3.988 12.861 12.861 1.7 0.2 0.2 0.2 0.205293
            4.623 13.657 13.657 1.52 1 0 0 0.171108
            2.699 13.649 13.649 1.7 0.2 0.2 0.2 0.228267
            2.635 14.214 14.214 1.7 0.2 0.2 0.2 0.236537
            1.278 14.874 14.874 1.7 0.2 0.2 0.2 0.227715
            3.742 15.229 15.229 1.7 0.2 0.2 0.2 0.273663
            3.26 11.871 11.871 1.55 0 0 1 0.214115
            3.161 11.652 11.652 1.7 0.2 0.2 0.2 0.229553
            4.562 11.533 11.533 1.7 0.2 0.2 0.2 0.235618
            4.889 12.172 12.172 1.52 1 0 0 0.242051
            2.355 10.372 10.372 1.7 0.2 0.2 0.2 0.241867
            2.151 10.095 10.095 1.7 0.2 0.2 0.2 0.254916
            1.274 10.866 10.866 1.7 0.2 0.2 0.2 0.256203
            2.883 9.103 9.103 1.7 0.2 0.2 0.2 0.24977
            1.132 10.659 10.659 1.7 0.2 0.2 0.2 0.200698
            2.755 8.887 8.887 1.7 0.2 0.2 0.2 0.228267
            1.88 9.668 9.668 1.7 0.2 0.2 0.2 0.237824
            1.762 9.475 9.475 1.52 1 0 0 0.2347
            5.391 10.718 10.718 1.55 0 0 1 0.228451
            6.767 10.483 10.483 1.7 0.2 0.2 0.2 0.268884
            7.632 11.733 11.733 1.7 0.2 0.2 0.2 0.2483
            8.309 12.133 12.133 1.52 1 0 0 0.278625
            7.384 9.366 9.366 1.7 0.2 0.2 0.2 0.259144
            8.882 9.085 9.085 1.7 0.2 0.2 0.2 0.334314
            9.195 8.719 8.719 1.7 0.2 0.2 0.2 0.346995
            9.288 7.953 7.953 1.7 0.2 0.2 0.2 0.365006
            7.607 12.343 12.343 1.55 0 0 1 0.208969
            8.414 13.524 13.524 1.7 0.2 0.2 0.2 0.218158
            8.091 14.742 14.742 1.7 0.2 0.2 0.2 0.198125
            8.994 15.402 15.402 1.52 1 0 0 0.189855
            8.311 13.947 13.947 1.7 0.2 0.2 0.2 0.228267
            8.861 15.359 15.359 1.7 0.2 0.2 0.2 0.26723
            9.085 12.976 12.976 1.7 0.2 0.2 0.2 0.247565
            6.809 15.032 15.032 1.55 0 0 1 0.18673
            6.414 16.217 16.217 1.7 0.2 0.2 0.2 0.229002
            6.524 16.172 16.172 1.7 0.2 0.2 0.2 0.28322
            6.576 17.219 17.219 1.52 1 0 0 0.287447
            5.012 16.628 16.628 1.7 0.2 0.2 0.2 0.187649
            4.96 17.046 17.046 1.8 1 1 0 0.185995
            6.581 14.972 14.972 1.55 0 0 1 0.310788
            6.703 14.864 14.864 1.7 0.2 0.2 0.2 0.348282
            """;

        public override string Generate()
        {
            return $$"""
            struct {{AtomStructN}} {
              vec3 coords;
              float vdw_radius;
              vec3 element_color;
              float b_factor;
            };
            
            // sdf for a sphere at sphere.coords with radius sphere.vdw_radius + r_probe
            float {{SDSphereFN}}(vec3 pos, {{AtomStructN}} sphere) {
              return length(pos - sphere.coords) - (sphere.vdw_radius + {{RProbe}});
            }
            
            // Outer sdf for SAS
            float {{SDOuterFN}}(vec3 pos, {{AtomStructN}}[{{AtomCount}}] spheres, int num_spheres) {
              float minimum = {{SDSphereFN}}(pos, spheres[0]);
              for (int i = 1; i < num_spheres; ++i) {
                minimum = min(minimum, {{SDSphereFN}}(pos, spheres[i]));
              }
              return minimum;
            }
            
            // Sort two elements
            {{AtomStructN}}[2] {{TwoSortFN}}({{AtomStructN}}[2] elements, vec3 pos) {
              if ({{SDSphereFN}}(pos, elements[1]) < {{SDSphereFN}}(pos, elements[0])) {
                atom temp = elements[0];
                elements[0] = elements[1];
                elements[1] = temp;
              }
              return elements;
            }
            
            // Check whether a point lies in a triangle
            bool {{InTriangleFN}}(vec3 p, vec3 t1, vec3 t2, vec3 t3) {
            
              // Translate everything such that p lies in the origin
              t1 -= p;
              t2 -= p;
              t3 -= p;
            
              // Compute side normals
              vec3 u = cross(t1, t2);
              vec3 v = cross(t2, t3);
              vec3 w = cross(t3, t1);
            
              if (dot(u, v) < 0)
                return false;
              if (dot(v, w) < 0)
                return false;
              else
                return true;
            }
            
            // Sort elements from closest to pos to furthest
            {{AtomStructN}}[{{AtomCount}}] {{SortElementsFN}}({{AtomStructN}}[{{AtomCount}}] elements, int num_spheres,
                                            vec3 pos) {
            
              // Sort spheres
              // TODO: I am not a 100% sure that if I need to check whether the closest
              // sphe is actually on the outside of the surface. I think this should
              // always be true because the vdw spheres should(?) not overlap
              {{AtomStructN}}[2] tested;
              for (int n = num_spheres - 1; n > 0; --n) {
                for (int i = 0; i < n; ++i) {
                  tested[0] = elements[i];
                  tested[1] = elements[i + 1];
                  tested = {{TwoSortFN}}(tested, pos);
                  elements[i] = tested[0];
                  elements[i + 1] = tested[1];
                }
              }
              return elements;
            }
            
            // Compute a potential surface point on a circular arc
            vec3 {{Case2FN}}(vec3 pos, {{AtomStructN}} c1, {{AtomStructN}} c2) {
              float d = length(
                  c1.coords -
                  c2.coords); // length of the connecting line between the sphere centers
              float r1 = c1.vdw_radius + {{RProbe}};
              float r2 = c2.vdw_radius + {{RProbe}};
              float d1 = (d * d - r2 * r2 + r1 * r1) /
                         (2 * d); // length of the line connecting one sphere center with m
              vec3 a = normalize(pos - c1.coords);
              vec3 b = normalize(c2.coords - c1.coords);
              vec3 m =
                  c1.coords + b * d1; // the point on the line connecting the two sphere
                                      // centers, perpendicular to the surface point
              float h = sqrt(r1 * r1 - d1 * d1); // distance from m to the surface point
              vec3 f = normalize(
                  a - dot(a, b) *
                          b); // Project the vector a on b -> vector parallel to the line
                              // connecting the sphere centers, by subtracting this parallel
                              // vector from a, we get a vector orthogonal to the  the
                              // connecting line that points in the direction of our current
                              // position -> this is the direction in which we need to walk
                              // from m to arrive at the surface point
              return m + f * h; // the SAS surface point
            }
            
            // Compute a potential surface point at an intersection of 3 spheres
            // This is basically a trilateration problem (see Fang1986
            // (https://arc.aiaa.org/doi/10.2514/3.20169))
            vec3[2] {{Case3FN}}(vec3 pos, {{AtomStructN}} c1, {{AtomStructN}} c2, {{AtomStructN}} c3) {
              vec3 base1 = c2.coords - c1.coords;
              float b1 = length(base1);
              vec3 base2 = c3.coords - c1.coords;
              float b2 = length(base2);
            
              vec3 i = normalize(base1);
              vec3 j = (base2 - dot(base2, i) * i);
              j = normalize(j);
              float r1 = c1.vdw_radius + {{RProbe}};
              float r2 = c2.vdw_radius + {{RProbe}};
              float r3 = c3.vdw_radius + {{RProbe}};
            
              float R0_2 = r1 * r1;
              float R1_2 = r2 * r2;
              float R2_2 = r3 * r3;
            
              float x = (R0_2 - R1_2 + b1 * b1) / (2 * b1);
              float y =
                  (R0_2 - R2_2 + b2 * b2 - 2 * dot(base2, i) * x) / (2 * dot(base2, j));
              float z = sqrt(R0_2 - x * x - y * y);
            
              vec3[2] xsas;
              xsas[0] = c1.coords + i * x + j * y + cross(i, j) * z;
            
              xsas[1] = c1.coords + i * x + j * y - cross(i, j) * z;
              return xsas;
            }

            {{AtomStructN}}[{{AtomCount}}] Atoms = {{AtomStructN}}[]({{string.Join(',', Atoms.Select(a => $"{AtomStructN}(vec3{a.Coords},{a.VDWRadius},vec3{a.ElementColor},{a.BFactor})"))}});
            
            
            {{(IsColorSDF ? $"vec3 {ColorOutputName};" : "")}}


            // Map the position onto a color and a distance to the SES
            // returns vec4(color, distance)
            
            // red for correspondence to spherical patch
            // yellow for correspondence to spherical arc
            // blue for correspondence intersection point
            float {{SDFName}}(vec3 pos) {
              {{AtomStructN}}[{{AtomCount}}] atoms = Atoms;
              {{AtomStructN}}[{{AtomCount}}] elements;
              int num_spheres = {{UsedAtoms}};
            
              int j = 0;
              for (int i = 0; i < num_spheres; i++) {
                // moved coordinates
                {{AtomStructN}} mv_atom = atoms[i];
            
                if (length(mv_atom.coords - pos) < 3 * {{RProbe}}) {
                  elements[j] = mv_atom;
                  ++j;
                }
              }
            
              // Calculate the outer SDF of the SAS
              num_spheres = j;
            
              if (num_spheres == 0) {
                {{(IsColorSDF ? $"{ColorOutputName} = vec3(0);" : "")}}
                return 2 * {{RProbe}};
              }
              float sdf = {{SDOuterFN}}(pos, elements, num_spheres);
              vec3 xsas;
              vec3 color;
            
              // Calculate the inner SDF of the SAS -> This is necessary for displaying the
              // SES If we are inside of the SAS we need to consider 3 different cases
              if (sdf < 0) {
            
                elements = {{SortElementsFN}}(elements, num_spheres, pos);
                // Case one: We are on a straight line segment to a spherical surface patch
                // --> Then the sdf is the same as for the outer region
            
                vec3 dir = normalize(pos - elements[0].coords);
                // Calculate the potential surface point of the closest atom
                xsas = elements[0].coords + (elements[0].vdw_radius + {{RProbe}}) * dir;
            
                color = elements[0].element_color;
            
                // if the calculated surface point is inside another sphere, our closest
                // point does not lie on a spherical patch, we need to consider case2 where
                // our closest point lies on a circular arc
                bool found = false;
                int iter = min({{K}}, num_spheres);
                if (abs({{SDOuterFN}}(xsas, elements, iter)) > {{GenerationManager.EPSILON_NAME}}) {
                  // Compute the potential surface points on the circular arc between two
                  // atoms for all atoms
                  for (int i = 0; i < iter - 1; i++) {
                    for (int j = i + 1; j < iter; j++) {
                      xsas = {{Case2FN}}(pos, elements[i], elements[j]);
                      // If we have found a true surface point,
                      // the triangle defined by the surface point and the sphere centers
                      // needs to include our position and the surface point needs to lie on
                      // the surface
                      if ({{InTriangleFN}}(pos, elements[i].coords, elements[j].coords, xsas) &&
                          abs({{SDOuterFN}}(xsas, elements, iter)) < {{GenerationManager.EPSILON_NAME}}) {
                        found = true;
                        break;
                      }
                    }
                    if (found)
                      break;
                  }
            
                  // If we could not find a surface point using case1 and case2, the closest
                  // point on the surface is an intersection point
                  if (!found) {
                    vec4 x_pot;
                    bool first = true;
                    int i = 0, j = 1, k = 2, l;
                    for (i = 0; i < iter - 2; i++) {
                      for (j = i + 1; j < iter - 1; j++) {
                        for (k = j + 1; k < iter; k++) {
                          vec3[2] xs = {{Case3FN}}(pos, elements[i], elements[j], elements[k]);
                          for (l = 0; l < 2; l++) {
                            // The potential point needs to lie on the surface
                            if (abs({{SDOuterFN}}(xs[l], elements, iter)) < {{GenerationManager.EPSILON_NAME}}) {
                              float dist = length(xs[l] - pos);
                              if (first) {
                                x_pot = vec4(xs[l], length(xs[l] - pos));
                                first = false;
                                // And its distance to pos needs to be smaller than for the
                                // other potential points
                              } else if (dist < x_pot.a) {
                                x_pot = vec4(xs[l], dist);
                              }
                            }
                          }
                        }
                      }
                    }
                    xsas = x_pot.xyz;
                  }
            
                  // Return the negative distance to the surface point
                  sdf = -1 * length(pos - xsas);
                }
              }

              {{(IsColorSDF ? $"{ColorOutputName} = color;" : "")}}
              return sdf + {{RProbe}};
            }
            """;
        }
    }
}
