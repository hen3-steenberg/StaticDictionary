import matplotlib.pyplot as plt

class data_row:
    
    def __init__(self, line : str):
        vals = line.split(",")
        self.element_count = int(vals[0].strip())
        self.description = vals[1].strip().replace("_", ",")
        self.static_duration = float(vals[2].strip())
        self.dynamic_duration = float(vals[3].strip())


def plot(description: str, results: list[data_row]):

    Elements = []
    Static_Duration = []
    Dynamic_Duration = []

    for row in results:
        Elements.append(row.element_count)
        Static_Duration.append(row.static_duration)
        Dynamic_Duration.append(row.dynamic_duration)
        
    plt.suptitle(description)
    plt.xlabel("Number of Elements")
    plt.ylabel("Run time [s]")
    plt.semilogx(Elements,Static_Duration, 'g-', label="Static")
    plt.semilogx(Elements, Dynamic_Duration, 'r-', label="Dynamic")
    plt.legend()
    plt.show()

RandomAccess_1 = []
RandomAccess_1000 = []
ContainsKey_1 = []
ContainsKey_1000 = []

results = open("Results.csv", 'r')
for line in results.readlines()[1::]:
    row = data_row(line)
    if row.description == "Random Access Test":
        RandomAccess_1.append(row)
    elif row.description == "Random Access Test, 1000 Runs.":
        RandomAccess_1000.append(row)
    elif row.description == "ContainsKey test 90% hit Test":
        ContainsKey_1.append(row)
    else:
        ContainsKey_1000.append(row)

plot("Random Access Test, 1000 Runs", RandomAccess_1000)
plot("Contains Key Test, 1000 Runs", ContainsKey_1000)