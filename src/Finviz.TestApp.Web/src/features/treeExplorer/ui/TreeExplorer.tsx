import {
    List,
    ListItemButton,
    ListItemText,
    Collapse,
    Typography,
    Box,
} from "@mui/material";
import { ExpandLess, ExpandMore } from "@mui/icons-material";
import { Spinner } from "@/shared/ui/Spinner";
import { useNodeChildren } from "../model/useNodeChildren";
import {
    forwardRef,
    useImperativeHandle,
    useState,
    useRef,
    memo
} from "react";
import { TreeConnector } from "./TreeConnector";

interface NodeProps {
    parentId: number | "root";
    depth: number;
    openMap: Record<number, boolean>;
    toggle: (id: number, state?: boolean) => void;
    selectedId?: number | null;
    onSelectNode?: (id: number) => void;
}

const NodeList = ({
                      parentId,
                      depth,
                      openMap,
                      toggle,
                      onSelectNode,
                      selectedId,
                  }: NodeProps) => {
    const { data, isLoading } = useNodeChildren(parentId);

    if (isLoading) return <Spinner />;

    if (!data || data.length === 0)
        return parentId === "root" ? <Typography>No nodes</Typography> : null;

    return (
        <List dense disablePadding sx={{ position: "relative" }}>
            {data.map((node) => (
                <Box key={node.id} sx={{ position: "relative" }}>
                    <TreeConnector depth={depth} hasParent={parentId !== "root"} />

                    <ListItemButton
                        data-node-id={node.id}
                        selected={node.id === selectedId}
                        onClick={() => {
                            if (node.hasChildren) toggle(node.id);
                            else onSelectNode?.(node.id);
                        }}
                        sx={{
                            pl: depth * 3,
                            "&.Mui-selected": {
                                bgcolor: "primary.main",
                                color: "white",
                                "&:hover": { bgcolor: "primary.dark" },
                            },
                        }}
                    >
                        <ListItemText
                            primary={node.name}
                            secondary={`Size: ${node.size}`}
                        />
                        {node.hasChildren &&
                            (openMap[node.id] ? <ExpandLess /> : <ExpandMore />)}
                    </ListItemButton>

                    {node.hasChildren && (
                        <Collapse in={openMap[node.id]} timeout="auto" unmountOnExit>
                            <Box pl={1}>
                                <NodeList
                                    parentId={node.id}
                                    depth={depth + 1}
                                    openMap={openMap}
                                    toggle={toggle}
                                    selectedId={selectedId}
                                    onSelectNode={onSelectNode}
                                />
                            </Box>
                        </Collapse>
                    )}
                </Box>
            ))}
        </List>
    );
};

export interface TreeExplorerRef {
    revealPath: (ids: number[]) => void;
}

export const TreeExplorer = memo(forwardRef<
    TreeExplorerRef,
    { onSelectNode?: (id: number) => void }
>(({ onSelectNode }, ref) => {
    const [openMap, setOpenMap] = useState<Record<number, boolean>>({});
    const [selectedId, setSelectedId] = useState<number | null>(null);

    const containerRef = useRef<HTMLDivElement | null>(null);

    const toggle = (id: number, state?: boolean) =>
        setOpenMap((prev) => ({ ...prev, [id]: state ?? !prev[id] }));

    useImperativeHandle(ref, () => ({
        async revealPath(ids: number[]) {
            setOpenMap(() => {
                const updated: Record<number, boolean> = {};
                ids.forEach((id) => (updated[id] = true));
                return updated;
            });

            const targetId = ids.at(-1) ?? null;
            setSelectedId(targetId);

            await new Promise((resolve) => setTimeout(resolve, 1000));

            const el = document.querySelector(`[data-node-id="${targetId}"]`);
            if (el) {
                el.scrollIntoView({ behavior: "smooth", block: "center" });
            }
        },
    }));

    return (
        <Box ref={containerRef}>
            <NodeList
                parentId="root"
                depth={1}
                openMap={openMap}
                toggle={toggle}
                selectedId={selectedId}
                onSelectNode={(id) => {
                    setSelectedId(id);
                    onSelectNode?.(id);
                }}
            />
        </Box>
    );
}));
