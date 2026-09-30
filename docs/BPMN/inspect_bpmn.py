import xml.etree.ElementTree as ET
import sys

sys.stdout.reconfigure(encoding='utf-8')

tree = ET.parse('docs/BPMN/diagrama_completo_lucroimpresso2.bpmn')
root = tree.getroot()

ns = {'bpmn': 'http://www.omg.org/spec/BPMN/20100524/MODEL'}

# Participants / Pools
for part in root.findall('.//bpmn:participant', ns):
    print(f"POOL: id={part.attrib.get('id')} name={repr(part.attrib.get('name'))} processRef={part.attrib.get('processRef')}")

print("\n=== MESSAGE FLOWS (between Pools) ===")
for mf in root.findall('.//bpmn:messageFlow', ns):
    print(f"MsgFlow id={mf.attrib.get('id')} name={repr(mf.attrib.get('name'))} source={mf.attrib.get('sourceRef')} target={mf.attrib.get('targetRef')}")

for process in root.findall('.//bpmn:process', ns):
    print(f"\n=== PROCESS: id={process.attrib.get('id')} name={repr(process.attrib.get('name'))}")
    
    # Lanes
    for lane in process.findall('.//bpmn:lane', ns):
        lane_name = lane.attrib.get('name')
        refs = [r.text for r in lane.findall('bpmn:flowNodeRef', ns)]
        print(f"\n  LANE: {lane_name}")
        for r in refs:
            el = process.find(f"*[@id='{r}']")
            tag = el.tag.split('}')[-1] if el is not None else '?'
            name = el.attrib.get('name') if el is not None else '?'
            print(f"    [{tag}] {r}: {repr(name)}")

        
    # Elements
    for child in process:
        tag = child.tag.split('}')[-1]
        name = child.attrib.get('name')
        cid = child.attrib.get('id')
        if tag not in ['laneSet', 'sequenceFlow', 'association']:
            print(f"  [{tag}] id={cid} | name={repr(name)}")

    print("  -- Flows:")
    for sf in process.findall('bpmn:sequenceFlow', ns):
        print(f"    {sf.attrib.get('sourceRef')} -> {sf.attrib.get('targetRef')} | name={repr(sf.attrib.get('name'))}")

