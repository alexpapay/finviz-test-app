import { useState } from "react";
import {
    List,
    ListItemButton,
    ListItemText,
    Collapse,
    Typography
} from "@mui/material";
import { ExpandLess, ExpandMore } from "@mui/icons-material";
import { useNodeChildren } from "../model/useNodeChildren";
import { Spinner } from "@/shared/ui/Spinner";

interface NodeProps {
    parentId: number | "root";
    onSelectNode?: (id: number) => void;
}

const NodeList = ({ parentId, onSelectNode }: NodeProps) => {
    const { data, isLoading } = useNodeChildren(parentId);
    const [open, setOpen] = useState<Record<number, boolean>>({});

    if (isLoading) return <Spinner />;
    if (!data || data.length === 0)
        return parentId === "root" ? (
            <Typography>No nodes</Typography>
        ) : null;

    const toggle = (id: number) =>
        setOpen((prev) => ({ ...prev, [id]: !prev[id] }));

    return (
        <List dense disablePadding>
            {data.map((node) => (
                <div key={node.id}>
                    <ListItemButton
                        onClick={() =>
                            node.hasChildren ? toggle(node.id) : onSelectNode?.(node.id)
                        }
                    >
                        <ListItemText
                            primary={node.name}
                            secondary={`Size: ${node.size}`}
                        />
                        {node.hasChildren &&
                            (open[node.id] ? <ExpandLess /> : <ExpandMore />)}
                    </ListItemButton>
                    {node.hasChildren && (
                        <Collapse in={open[node.id]} timeout="auto" unmountOnExit>
                            <div style={{ paddingLeft: 16 }}>
                                <NodeList parentId={node.id} onSelectNode={onSelectNode} />
                            </div>
                        </Collapse>
                    )}
                </div>
            ))}
        </List>
    );
};

export const TreeExplorer = ({ onSelectNode }: { onSelectNode?: (id: number) => void }) => (
    <NodeList parentId="root" onSelectNode={onSelectNode} />
);
