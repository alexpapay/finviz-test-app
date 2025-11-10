import { Box } from "@mui/material";
import { useTheme } from "@mui/material/styles";

interface TreeConnectorProps {
    depth: number;
    hasParent?: boolean;
}

const NODE_INDENT = 20;
const LINE_OFFSET = 16;

export const TreeConnector: React.FC<TreeConnectorProps> = ({ depth, hasParent = true,}) => {
    const theme = useTheme();
    const color =
        theme.palette.mode === "dark"
            ? "rgba(255,255,255,0.15)"
            : "rgba(0,0,0,0.15)";

    if (!hasParent) return null;

    return (
        <Box
            sx={{
                position: "absolute",
                top: 0,
                left: depth * NODE_INDENT - LINE_OFFSET,
                bottom: 0,
                width: "16px",
                pointerEvents: "none",
                "& svg": {
                    position: "absolute",
                    width: "16px",
                    height: "100%",
                },
            }}
        >
            <svg viewBox="0 0 16 100" preserveAspectRatio="none">
                <path d="M8 0 v100" stroke={color} strokeWidth="1" fill="none" />
            </svg>
        </Box>
    );
};
